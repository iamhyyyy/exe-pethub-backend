using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IAppointmentReminderRepository : IGenericRepository<AppointmentReminder>
    {
        Task <AppointmentReminder> GetByAppointmentIdAsync(string appointmentId);
        Task<List<AppointmentReminder>> GetAllAsync();
        Task<List<AppointmentReminder>> GetPendingForSendAsync(DateTime now);
    }
}