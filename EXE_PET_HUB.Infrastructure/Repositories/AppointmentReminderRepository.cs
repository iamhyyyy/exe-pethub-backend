using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Domain.Enums;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class AppointmentReminderRepository : GenericRepository<AppointmentReminder>, IAppointmentReminderRepository
    {
        private readonly AppDbContext _context;
        public AppointmentReminderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<AppointmentReminder> GetByAppointmentIdAsync(string appointmentId)
        {
            return await _context.AppointmentReminders.FirstOrDefaultAsync(r => r.AppointmentId == appointmentId);
        }
        public async Task<List<AppointmentReminder>> GetAllAsync()
        {
            return await _context.AppointmentReminders.Include(r => r.Appointment).ToListAsync();
        }

        public async Task<List<AppointmentReminder>> GetPendingForSendAsync(DateTime now)
        {
            return await _context.AppointmentReminders
                .Include(r => r.Appointment)
                    .ThenInclude(a => a.Customer)
                .Where(r => r.Status == ReminderStatus.Pending && r.ReminderTime <= now)
                .ToListAsync();
        }
    } 
}