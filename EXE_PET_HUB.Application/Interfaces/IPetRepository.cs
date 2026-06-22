using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IPetRepository : IStoreGenericRepository<Pet>
    {
        Task<List<Pet>> GetByCustomerIdAsyncAndStoreId(Guid customerId, string storeId);

    }
}