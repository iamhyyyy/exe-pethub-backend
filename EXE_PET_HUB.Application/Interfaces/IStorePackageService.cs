using EXE_PET_HUB.Application.DTOs.StorePackage;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IStorePackageService
    {
        /// <summary>
        /// Tạo bản ghi StorePackagePayment mới với Status = Pending
        /// </summary>
        Task<StorePackagePaymentDto> CreateAsync(CreateStorePackageDto dto);

        /// <summary>
        /// Lấy danh sách gói đã mua của Manager
        /// </summary>
        Task<List<StorePackagePaymentDto>> GetByManagerIdAsync(Guid managerId);

        /// <summary>
        /// Lấy chi tiết 1 gói theo Id
        /// </summary>
        Task<StorePackagePaymentDto?> GetByIdAsync(string id);

        /// <summary>
        /// Lấy tất cả danh sách gói đã mua
        /// </summary>
        Task<List<StorePackagePaymentDto>> GetAllAsync();
    }
}
