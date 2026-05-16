using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IAppointmentReminderRepository : IGenericRepository<AppointmentReminder>
    {
        Task <AppointmentReminder> GetByAppointmentIdAsync(string appointmentId);
        Task<List<AppointmentReminder>> GetAllAsync();
    }
}