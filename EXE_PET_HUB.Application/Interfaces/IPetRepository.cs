using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IPetRepository : IGenericRepository<Pet>
    {
        Task<List<Pet>> GetByCustomerIdAsync(Guid customerId);

    }
}