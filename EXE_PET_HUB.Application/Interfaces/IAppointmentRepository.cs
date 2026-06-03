using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<List<Appointment>> GetByCustomerIdAsync(Guid customerId);

        Task<List<Appointment>> GetByPetIdAsync(string petId);

        Task<List<Appointment>> GetConfirmedAppointments();

    }
}