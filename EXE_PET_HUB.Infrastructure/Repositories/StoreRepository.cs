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
    }
}