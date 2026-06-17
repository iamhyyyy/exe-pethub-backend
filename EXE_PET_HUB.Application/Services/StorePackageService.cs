using EXE_PET_HUB.Application.DTOs.StorePackage;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Application.Services
{
    public class StorePackageService : IStorePackageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StorePackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StorePackagePaymentDto> CreateAsync(Guid managerId, CreateStorePackageDto dto)
        {
            // Validate Manager tồn tại
            var manager = await _unitOfWork.UserRepository.GetByIdAsync(managerId);
            if (manager == null)
                throw new Exception($"Không tìm thấy Manager với Id '{managerId}'.");

            // Validate PlatformPlan tồn tại và đang active
            var plan = await _unitOfWork.PlatformPlanRepository.GetByIdAsync(dto.PlanId);
            if (plan == null)
                throw new Exception($"Không tìm thấy gói (PlatformPlan) với Id '{dto.PlanId}'.");

            if (!plan.IsActive)
                throw new Exception($"Gói '{plan.Name}' hiện không còn khả dụng.");

            var package = new StorePackagePayment
            {
                Id = Guid.NewGuid().ToString(),
                ManagerId = managerId,
                PackageType = plan.Name,
                Price = (double)plan.Price,
                DurationInDays = plan.DurationInDays,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddHours(7),
                UpdatedAt = DateTime.UtcNow.AddHours(7)
            };

            await _unitOfWork.StorePackageRepository.AddAsync(package);
            await _unitOfWork.CompleteAsync();

            return MapToDto(package, manager);
        }

        public async Task<List<StorePackagePaymentDto>> GetByManagerIdAsync(Guid managerId)
        {
            var packages = await _unitOfWork.StorePackageRepository.GetAllByManagerIdAsync(managerId);
            return packages.Select(p => MapToDto(p, p.Manager)).ToList();
        }

        public async Task<List<StorePackagePaymentDto>> GetAllAsync()
        {
            var packages = await _unitOfWork.StorePackageRepository.GetAllStorePackagesAsync();
            return packages.Select(p => MapToDto(p, p.Manager)).ToList();
        }

        public async Task<StorePackagePaymentDto?> GetByIdAsync(string id)
        {
            var package = await _unitOfWork.StorePackageRepository.GetByIdAsync(id);
            if (package == null) return null;

            var manager = await _unitOfWork.UserRepository.GetByIdAsync(package.ManagerId);
            return MapToDto(package, manager!);
        }

        private static StorePackagePaymentDto MapToDto(StorePackagePayment p, User manager)
        {
            return new StorePackagePaymentDto
            {
                Id = p.Id,
                ManagerId = p.ManagerId,
                ManagerName = manager?.UserName ?? string.Empty,
                PackageType = p.PackageType,
                Price = p.Price,
                DurationInDays = p.DurationInDays,
                PayOsOrderCode = p.PayOsOrderCode,
                Status = p.Status.ToString(),
                PaymentMethod = p.PaymentMethod,
                TransactionNo = p.TransactionNo,
                PaidAt = p.PaidAt,
                CreatedAt = p.CreatedAt
            };
        }
    }
}
