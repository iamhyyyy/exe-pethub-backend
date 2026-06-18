using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class PetRepository : StoreGenericRepository<Pet>, IPetRepository
    {
        //private readonly AppDbContext _context;
        public PetRepository(AppDbContext context) : base(context)
        {
            //_context = context;
        }
        public async Task<List<Pet>> GetByCustomerIdAsyncAndStoreId(Guid customerId, string storeId)
        {
            return await _context.Pets
                .Where(p => p.CustomerId == customerId && p.StoreId == storeId)
                .ToListAsync();

        }
    }
}