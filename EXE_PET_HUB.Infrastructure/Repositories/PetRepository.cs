using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class PetRepository : GenericRepository<Pet>, IPetRepository
    {
        public PetRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}