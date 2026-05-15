using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Appointment?> GetByIdAsync(string id)
        {
            return await _context.Appointments
                .Include(p => p.Customer)
                .Include(p => p.Pet)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}