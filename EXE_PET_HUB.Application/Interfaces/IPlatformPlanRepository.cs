using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IPlatformPlanRepository : IGenericRepository<PlatformPlan>
    {
        // Chỉ lấy các gói đang active (dùng cho manager/customer xem)
        Task<List<PlatformPlan>> GetAllActiveAsync();
    }
}
