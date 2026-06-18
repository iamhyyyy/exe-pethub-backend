using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<List<MedicalRecord>> GetByPetIdAsync(string petId)
        {
            return await _context.MedicalRecords
                .Where(r => r.PetId == petId).ToListAsync();
        }

        public async Task<List<MedicalRecord>> GetByAppointmentIdAsync(string appointmentId)
        {
            return await _context.MedicalRecords
                .Where(r => r.AppointmentId == appointmentId).ToListAsync();
        }
    }
}
