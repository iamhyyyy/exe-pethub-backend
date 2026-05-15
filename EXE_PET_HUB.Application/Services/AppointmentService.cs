using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.DTOs;
using AutoMapper;
using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Application.Services
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        string statusColor = "#333"; // Mặc định
        AppointmentStatus statusText = AppointmentStatus.Confirmed;
        string customMessage = "We are writing to update you on your appointment status.";

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _emailService = emailService;
        }

        public async Task<List<AppointmentDto>> GetAllAsync()
        {
            var appointments = await _unitOfWork.AppointmentRepository.GetAllAsync();

            return _mapper.Map<List<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto?> GetByIdAsync(string id)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);

            return appointment == null ? null : _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<List<AppointmentDto>> GetByPetIdAsync(string petId)
        {
            var appointments = await _unitOfWork.AppointmentRepository.GetByPetIdAsync(petId);
            return _mapper.Map<List<AppointmentDto>>(appointments);
        }

        public async Task<List<AppointmentDto>> GetByCustomerIdAsync(Guid customerId)
        {
            var appointments = await _unitOfWork.AppointmentRepository.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto> CreateAsync(CreateAppointmentDto dto)
        {
            var appointment = _mapper.Map<Appointment>(dto);
            appointment.Id = Guid.NewGuid().ToString();
            appointment.Status = AppointmentStatus.Confirmed;
            appointment.CreatedAt = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.AppointmentRepository.AddAsync(appointment);
            await _unitOfWork.CompleteAsync();

            appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(appointment.Id);
            var emailBody = WriteEmailContent(appointment);
            await _emailService.SendEmailAsync(appointment.Customer.Email, "Appointment Confirmation for " + appointment.Pet.Name, emailBody);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<bool> Update(string id, UpdateAppointmentDto dto)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null) return false;

            _mapper.Map(dto, appointment);
            appointment.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.CompleteAsync();

            appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            var emailBody = WriteEmailContent(appointment);
            await _emailService.SendEmailAsync(appointment.Customer.Email, "Appointment Update for " + appointment.Pet.Name, emailBody);

            return true;
        }

        public async Task<bool> Delete(string id)
        {
            var appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            if (appointment == null) return false;

            appointment.Status = AppointmentStatus.Cancelled;

            _unitOfWork.AppointmentRepository.Update(appointment);
            await _unitOfWork.CompleteAsync();

            appointment = await _unitOfWork.AppointmentRepository.GetByIdAsync(id);
            var emailBody = WriteEmailContent(appointment);
            await _emailService.SendEmailAsync(appointment.Customer.Email, "Appointment Cancellation for " + appointment.Pet.Name, emailBody);

            return true;
        }

        public string WriteEmailContent(Appointment appointment)
        {
            switch (appointment.Status)
            {
                case AppointmentStatus.Confirmed:
                    statusColor = "#28a745"; // Xanh lá
                    customMessage = "Great news! Your appointment has been confirmed. We look forward to seeing you and your pet.";
                    break;
                case AppointmentStatus.Cancelled:
                    statusColor = "#dc3545"; // Đỏ
                    customMessage = "We regret to inform you that your appointment has been cancelled. Please contact us if you wish to reschedule.";
                    break;
                case AppointmentStatus.Completed:
                    statusColor = "#007bff"; // Xanh dương (Professional & Trust)
                    customMessage = "Thank you for choosing Pet Hub! Your pet's visit is now complete. We hope you had a great experience.";
                    break;
            }

            string emailBody = $@"
<div style='font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 15px; overflow: hidden; box-shadow: 0 4px 10px rgba(0,0,0,0.1);'>
    <div style='background: linear-gradient(135deg, #4CAF50 0%, #2E7D32 100%); color: white; padding: 30px; text-align: center;'>
        <h1 style='margin: 0; font-size: 24px;'>🐾 PET HUB</h1>
        <p style='margin: 5px 0 0 0; opacity: 0.9;'>Your Pet's Health, Our Priority</p>
    </div>

    <div style='padding: 30px;'>
        <h2 style='color: #2E7D32; margin-top: 0;'>Hello {appointment.Customer.FirstName} {appointment.Customer.LastName},</h2>
        <p style='font-size: 16px;'>{customMessage}</p>
        
        <div style='background-color: #f8fbf8; padding: 20px; border-left: 4px solid #4CAF50; border-radius: 8px; margin: 25px 0;'>
            <table style='width: 100%; border-collapse: collapse;'>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Pet Name:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.Pet.Name}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Date:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.AppointmentDate:dddd, MMM dd, yyyy}</td>
                </tr>
                <tr>
                    <td style='padding: 8px 0; color: #666;'><strong>Time:</strong></td>
                    <td style='padding: 8px 0; text-align: right;'>{appointment.StartTime} - {appointment.EndTime}</td>
                </tr>
                <tr>
                    <td style='padding: 20px 0 8px 0; border-top: 1px solid #eee;'><strong>Current Status:</strong></td>
                    <td style='padding: 20px 0 8px 0; text-align: right; border-top: 1px solid #eee;'>
                        <span style='background-color: {statusColor}; color: white; padding: 5px 15px; border-radius: 20px; font-size: 14px; font-weight: bold;'>
                            {appointment.Status}
                        </span>
                    </td>
                </tr>
            </table>
        </div>

        {(appointment.Status == AppointmentStatus.Completed
            ? "<p style='text-align: center; font-style: italic; color: #666;'>How was your experience? We'd love to hear your feedback!</p>"
            : "")}

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