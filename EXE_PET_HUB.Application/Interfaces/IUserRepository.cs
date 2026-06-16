using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByIdAsync(Guid id);
        Task<List<User>> GetAllCustomersByStoreIdAsync(string storeId);
    }
}