using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IMedicalRecordRepository : IGenericRepository<MedicalRecord>
    {
        Task<List<MedicalRecord>> GetByPetIdAsync(string petId);
        Task<List<MedicalRecord>> GetByAppointmentIdAsync(string appointmentId);
    }
}
