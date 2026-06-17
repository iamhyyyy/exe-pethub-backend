using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class PlatformPlanRepository : GenericRepository<PlatformPlan>, IPlatformPlanRepository
    {
        private readonly AppDbContext _context;

        public PlatformPlanRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<PlatformPlan>> GetAllActiveAsync()
        {
            return await _context.PlatformPlans
                .Where(p => p.IsActive)
                .OrderBy(p => p.Price)
                .ToListAsync();
        }
    }
}
