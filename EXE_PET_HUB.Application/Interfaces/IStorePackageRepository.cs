using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IStorePackageRepository : IGenericRepository<StorePackagePayment>
    {
        Task<StorePackagePayment?> GetByOrderCodeAsync(long orderCode);
        Task<List<StorePackagePayment>> GetAllByManagerIdAsync(Guid managerId);
        Task<List<StorePackagePayment>> GetAllStorePackagesAsync();
    }
}
