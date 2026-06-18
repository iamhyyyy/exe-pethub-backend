using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IAppointmentRepository : IStoreGenericRepository<Appointment>
    {
        //Task<Appointment?> GetByIdAsyncByStoreId(string id, string storeId);

        Task<List<Appointment>> GetByCustomerIdAsyncAndStoreId(Guid customerId, string storeId);

        Task<List<Appointment>> GetByPetIdAsyncAndStoreId(string petId, string storeId);

        Task<List<Appointment>> GetConfirmedAppointments();

    }
}