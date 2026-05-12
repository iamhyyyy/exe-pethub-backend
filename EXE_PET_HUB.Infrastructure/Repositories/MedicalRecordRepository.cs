using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        private readonly AppDbContext _context;

        public MedicalRecordRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<MedicalRecord>> GetAllAsync()
        {
            return await _context.MedicalRecords.ToListAsync();
        }

        public async Task<MedicalRecord?> GetByIdAsync(string id)
        {
            return await _context.MedicalRecords.FindAsync(id);
        }

        public Task AddAsync(MedicalRecord record)
        {
            _context.MedicalRecords.Add(record);
            return Task.CompletedTask;
        }

        public new void Update(MedicalRecord record)
        {
            _context.MedicalRecords.Update(record);
        }
    }
}
