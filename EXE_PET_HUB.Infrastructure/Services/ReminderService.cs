using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EXE_PET_HUB.Infrastructure.Services
{

    public class ReminderService : IReminderService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReminderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private static DateTime ToAppointmentLocal(Appointment appointment) =>
    appointment.AppointmentDate.ToDateTime(appointment.StartTime);

        public async Task CancelExpiredAppointmentsAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReminderService>>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var nowEnvn = DateTime.UtcNow.AddHours(7);

            try
            {
                logger.LogInformation("Bắt đầu quét lịch hẹn quá hạn 2 tiếng (Giờ VN: {Now})...", nowEnvn);

                var confirmedAppointments = await unitOfWork.AppointmentRepository.GetConfirmedAppointments();

                var expiredAppointments = confirmedAppointments
                    .Where(a => nowEnvn >= ToAppointmentLocal(a).AddHours(2))
                    .ToList();

                if (expiredAppointments.Any())
                {
                    foreach (var appointment in expiredAppointments)
                    {
                        appointment.Status = AppointmentStatus.Cancelled;
                        appointment.UpdatedAt = nowEnvn;

                        unitOfWork.AppointmentRepository.Update(appointment);
                        logger.LogInformation("Tự động hủy lịch hẹn {AppointmentId} do quá hạn 2 tiếng.", appointment.Id);

                        // 2. Gửi email thông báo hủy lịch cho khách hàng
                        var customerEmail = appointment.Customer.Email;
                        if (!string.IsNullOrWhiteSpace(customerEmail))
                        {
                            try
                            {
                                var petName = appointment.Pet?.Name ?? "your pet";
                                var subject = $"[Pet Hub] Notice: Your appointment for {petName} has been cancelled";
                                var emailBody = WriteCancelEmailContent(appointment);

                                await emailService.SendEmailAsync(customerEmail, subject, emailBody);
                                logger.LogInformation("Đã gửi mail thông báo hủy lịch tới {Email}.", customerEmail);
                            }
                            catch (Exception mailEx)
                            {
                                // Nếu gửi mail lỗi thì log lại chứ không làm sập vòng lặp hủy các lịch tiếp theo
                                logger.LogError(mailEx, "Lỗi khi gửi mail thông báo hủy cho lịch hẹn {AppointmentId}", appointment.Id);
                            }
                        }
                    }

                    await unitOfWork.CompleteAsync();
                    logger.LogInformation("Đã tự động hủy thành công {Count} lịch hẹn quá hạn.", expiredAppointments.Count);
                }
                else
                {
                    logger.LogInformation("Không có lịch hẹn nào bị quá hạn 2 tiếng.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Có lỗi xảy ra trong quá trình tự động hủy lịch hẹn quá hạn.");
            }
        }

        public async Task SyncRemindersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var now = DateTime.UtcNow.AddHours(7);
            var limitTime = now.AddHours(-1);

            var existingReminderIds = (await unitOfWork.AppointmentReminderRepository.GetAllAsync())
                .Select(r => r.AppointmentId)
                .ToHashSet();

            var appointmentsToCreate = (await unitOfWork.AppointmentRepository.GetAllAsync())
                .Where(a => a.Status == AppointmentStatus.Confirmed)
                .Where(a => !existingReminderIds.Contains(a.Id))
                .Where(a => ToAppointmentLocal(a) >= limitTime)
                .ToList();

            foreach (var app in appointmentsToCreate)
            {
                var appointmentDateTime = ToAppointmentLocal(app);

                var reminder = new AppointmentReminder
                {
                    AppointmentId = app.Id,
                    ReminderTime = appointmentDateTime.AddDays(-1),
                    Status = ReminderStatus.Pending, // Hoặc enum tương đương của cậu
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };
                await unitOfWork.AppointmentReminderRepository.AddAsync(reminder);
            }

            if (appointmentsToCreate.Count > 0)
            {
                await unitOfWork.CompleteAsync();
            }
        }

        public async Task SendPendingRemindersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ReminderService>>();

            var now = DateTime.UtcNow.AddHours(7);
            var pendingReminders = await unitOfWork.AppointmentReminderRepository.GetPendingForSendAsync(now);

            foreach (var reminder in pendingReminders)
            {
                var appointment = reminder.Appointment;
                var customerEmail = appointment.Customer.Email;

                if (string.IsNullOrWhiteSpace(customerEmail))
                {
                    logger.LogWarning("Skip reminder {ReminderId}: customer has no email", reminder.Id);
                    continue;
                }

                try
                {
                    var petName = appointment.Pet?.Name ?? "your pet";
                    var subject = $"Pet care appointment reminder for {petName}";
                    var emailBody = WriteEmailContent(appointment);

                    await emailService.SendEmailAsync(customerEmail, subject, emailBody);

                    reminder.Status = ReminderStatus.Sent;
                    unitOfWork.AppointmentReminderRepository.Update(reminder);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send reminder {ReminderId} to {Email}", reminder.Id, customerEmail);
                    reminder.Status = ReminderStatus.Failed;
                    unitOfWork.AppointmentReminderRepository.Update(reminder);
                }
            }

            if (pendingReminders.Count > 0)
            {
                await unitOfWork.CompleteAsync();
            }
        }

        public string WriteCancelEmailContent(Appointment appointment)
        {
            string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 15px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>
    <div style='background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%); color: white; padding: 30px; text-align: center;'>
        <h1 style='margin: 0; font-size: 24px;'>🐾 PET HUB</h1>
        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Your Pet's Health, Our Priority</p>
    </div>

    <div style='padding: 30px;'>
        <h2 style='color: #2E7D32; margin-top: 0;'>Hello {appointment.Customer?.FirstName ?? ""} {appointment.Customer?.LastName ?? ""},</h2>
        <p style='font-size: 16px;'>We are writing to inform you that your appointment has been <strong>automatically cancelled</strong> because it was past the scheduled time by more than 2 hours without check-in.</p>
        
        <div style='background-color: #f8fbf8; padding: 20px; border-left: 4px solid #4CAF50; border-radius: 8px; margin: 25px 0;'>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Pet Name:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.Pet?.Name ?? "N/A"}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Scheduled Date:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.AppointmentDate:dddd, MMM dd, yyyy}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Scheduled Time:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.StartTime} - {appointment.EndTime}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Status:</strong></td>
                    <td style='padding: 8px 0; text-align: right; color: #d32f2f; font-weight: bold;'>Cancelled (Expired)</td>
                </tr>
            </table>
        </div>

        <p style='font-size: 14px; color: #555;'>If this was a mistake or you still need care for your pet, please feel free to book a new appointment on our platform or contact our hotline.</p>

        <div style='text-align: center; margin-top: 30px;'>
            <a href='https://pethub.com/appointments' style='background-color: #2E7D32; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>Book New Appointment</a>
        </div>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px; text-align: center; font-size: 12px; color: #999;'>
        <p>This is an automated system message. Please do not reply directly to this email.</p>
        <p><strong>Pet Hub System</strong><br>Dĩ An, Bình Dương, Vietnam | Hotline: 1900-PET-HUB</p>
    </div>
</div>";
            return emailBody;
        }

        public string WriteEmailContent(Appointment appointment)
        {

            string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 15px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>
    <div style='background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%); color: white; padding: 30px; text-align: center;'>
        <h1 style='margin: 0; font-size: 24px;'>🐾 PET HUB</h1>
        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Your Pet's Health, Our Priority</p>
    </div>

    <div style='padding: 30px;'>
        <h2 style='color: #2E7D32; margin-top: 0;'>Hello {appointment.Customer.FirstName ?? ""} {appointment.Customer.LastName ?? ""},</h2>
        <p style='font-size: 16px;'>You have an upcoming appointment with us. Here are the details:</p>
        
        <div style='background-color: #f8fbf8; padding: 20px; border-left: 4px solid #4CAF50; border-radius: 8px; margin: 25px 0;'>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Pet Name:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.Pet?.Name ?? "N/A"}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Date:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.AppointmentDate:dddd, MMM dd, yyyy}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Time:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.StartTime} - {appointment.EndTime}</td>
                </tr>
            </table>
        </div>

        <div style='text-align: center; margin-top: 30px;'>
            <a href='https://pethub.com/appointments/{appointment.Id}' style='background-color: #4CAF50; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold; display: inline-block;'>View Details On Website</a>
        </div>
    </div>

    <div style='background-color: #f4f4f4; padding: 20px; text-align: center; font-size: 12px; color: #999;'>
        <p>You received this email because you made an appointment at Pet Hub.</p>
        <p><strong>Pet Hub System</strong><br>Dĩ An, Bình Dương, Vietnam | Hotline: 1900-PET-HUB</p>
    </div>
</div>";

            return emailBody;
        }
    }
}
