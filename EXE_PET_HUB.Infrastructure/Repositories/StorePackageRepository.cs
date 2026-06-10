using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class StorePackageRepository : GenericRepository<StorePackagePayment>, IStorePackageRepository
    {
        private readonly AppDbContext _context;

        public StorePackageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<StorePackagePayment?> GetByOrderCodeAsync(long orderCode)
        {
            return await _context.StorePackagePayments
                .Include(s => s.Manager)
                .Where(s => s.PayOsOrderCode == orderCode)
                .SingleOrDefaultAsync();
        }

        public async Task<List<StorePackagePayment>> GetAllByManagerIdAsync(Guid managerId)
        {
            return await _context.StorePackagePayments
                .Include(s => s.Manager)
                .Where(s => s.ManagerId == managerId)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<StorePackagePayment>> GetAllStorePackagesAsync()
        {
            return await _context.StorePackagePayments
                .Include(s => s.Manager)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync();
        }
    }
}
