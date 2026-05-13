using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class MedicalRecordRepository : GenericRepository<MedicalRecord>, IMedicalRecordRepository
    {
        public MedicalRecordRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}
