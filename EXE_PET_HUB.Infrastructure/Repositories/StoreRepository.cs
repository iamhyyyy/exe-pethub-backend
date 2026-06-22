using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class StoreRepository : GenericRepository<Store>, IStoreRepository
    {
        public StoreRepository(AppDbContext context) : base(context)
        {
            
        }

        public async Task<List<Store>> GetAllAsync()
        {
            return await _context.Stores
                .Where(p => p.Id != "44444444-4444-4444-4444-444444444444")
                .ToListAsync();
        }
    }
}