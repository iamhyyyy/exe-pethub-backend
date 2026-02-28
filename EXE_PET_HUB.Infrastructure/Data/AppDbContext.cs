using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Identity;

namespace EXE_PET_HUB.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Pet> Pets { get; set; }
    }
}