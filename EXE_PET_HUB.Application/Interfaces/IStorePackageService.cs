using EXE_PET_HUB.Application.DTOs.StorePackage;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IStorePackageService
    {

        Task<StorePackagePaymentDto> CreateAsync(Guid managerId, CreateStorePackageDto dto);

        Task<List<StorePackagePaymentDto>> GetByManagerIdAsync(Guid managerId);


        Task<StorePackagePaymentDto?> GetByIdAsync(string id);

        Task<List<StorePackagePaymentDto>> GetAllAsync();
    }
}
