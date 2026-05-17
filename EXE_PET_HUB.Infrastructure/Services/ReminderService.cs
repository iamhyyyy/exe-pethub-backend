using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Infrastructure.Services
{

    public class ReminderService : IReminderService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public ReminderService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task SyncRemindersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            // Thay bằng tên DbContext hoặc Repository thực tế của cậu
            var appointmentRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<Appointment>>();
            var reminderRepo = scope.ServiceProvider.GetRequiredService<IGenericRepository<AppointmentReminder>>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>(); // Nếu cậu có UOW

            var now = DateTime.UtcNow.AddHours(7);
            var limitTime = now.AddHours(-5);

            // 1. Lấy danh sách Id của các Appointment đã có trong table Reminder
            var existingReminderIds = (await reminderRepo.GetAllAsync())
                .Select(r => r.AppointmentId)
                .ToHashSet();

            // 2. Lấy các Appointment thỏa mãn điều kiện:
            // - Chưa tồn tại trong table Reminder
            // - Thời gian hẹn chưa quá 5 tiếng so với hiện tại
            var appointmentsToCreate = (await appointmentRepo.GetAllAsync())
                .Where(a => !existingReminderIds.Contains(a.Id))
                .AsEnumerable() // Chuyển về Memory để xử lý DateOnly/TimeOnly nếu cần
                .Where(a => {
                    var appointmentDateTime = a.AppointmentDate.ToDateTime(a.StartTime);
                    return appointmentDateTime >= limitTime;
                })
                .ToList();

            // 3. Add thêm vào table AppointmentReminder
            foreach (var app in appointmentsToCreate)
            {
                var appointmentDateTime = app.AppointmentDate.ToDateTime(app.StartTime);

                var reminder = new AppointmentReminder
                {
                    AppointmentId = app.Id,
                    // Cậu có thể tùy chỉnh nhắc trước bao nhiêu tiếng, ví dụ nhắc trước 1 tiếng:
                    ReminderTime = appointmentDateTime.AddHours(-1),
                    Status = ReminderStatus.Pending, // Hoặc enum tương đương của cậu
                    CreatedAt = DateTime.UtcNow.AddHours(7)
                };
                await reminderRepo.AddAsync(reminder);
            }

            if (appointmentsToCreate.Any())
            {
                await unitOfWork.CompleteAsync(); // Lưu xuống DB
            }
        }

        public async Task SendPendingRemindersAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var reminderRepo = scope.ServiceProvider.GetRequiredService<IAppointmentReminderRepository>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

            var now = DateTime.UtcNow.AddHours(7);
            var pendingReminders = await reminderRepo.GetPendingForSendAsync(now);

            foreach (var reminder in pendingReminders)
            {
                try
                {
                    string customerEmail = reminder.Appointment.Customer.Email; // Đảm bảo bảng User có trường này
                    string subject = "Nhắc nhở lịch hẹn chăm sóc thú cưng!";
                    string body = $"Xin chào, bạn có lịch hẹn vào lúc {reminder.Appointment.StartTime} ngày {reminder.Appointment.AppointmentDate}.";

                    await emailService.SendEmailAsync(customerEmail, subject, body);

                    // Gửi xong thì cập nhật trạng thái
                    reminder.Status = ReminderStatus.Sent;
                    reminderRepo.Update(reminder);
                }
                catch (Exception)
                {
                    reminder.Status = ReminderStatus.Failed;
                    reminderRepo.Update(reminder);
                }
            }

            if (pendingReminders.Any())
            {
                await unitOfWork.CompleteAsync();
            }
        }
    }
}
