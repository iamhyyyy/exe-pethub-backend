using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class AppointmentRepository : StoreGenericRepository<Appointment>, IAppointmentRepository
    {
        //private readonly AppDbContext _context;
        public AppointmentRepository(AppDbContext context) : base(context)
        {
            //_context = context;
        }

        public async Task<Appointment?> GetByIdAsync(string id)
        {
            return await _context.Appointments
                .Include(p => p.Customer)
                .Include(p => p.Pet)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Appointment?> GetByIdAsyncByStoreId(string id, string storeId)
        {
            return await _context.Appointments
                .Include(p => p.Customer)
                .Include(p => p.Pet)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.StoreId == storeId);
        }

        public async Task<List<Appointment>> GetByCustomerIdAsyncAndStoreId(Guid customerId, string storeId)
        {
            return await _context.Appointments
                .Where(a => a.CustomerId == customerId && a.StoreId == storeId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetByPetIdAsyncAndStoreId(string petId, string storeId)
        {
            return await _context.Appointments
                .Where(a => a.PetId == petId && a.StoreId == storeId)
                .ToListAsync();
        }

        public async Task<List<Appointment>> GetConfirmedAppointments()
        {
            return await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Confirmed)
                .Include(p => p.Customer)
                .Include(p => p.Pet)
                .ToListAsync();
        }
    }
}