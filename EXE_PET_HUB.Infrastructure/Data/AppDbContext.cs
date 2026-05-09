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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Pet>().HasData(
                new Pet { Id = 1, Name = "Mochi",   Species = "Chó",     Age = 2, Price = 5000000 },
                new Pet { Id = 2, Name = "Kitty",   Species = "Mèo",     Age = 1, Price = 3000000 },
                new Pet { Id = 3, Name = "Buddy",   Species = "Chó",     Age = 3, Price = 7000000 },
                new Pet { Id = 4, Name = "Coco",    Species = "Mèo",     Age = 2, Price = 4000000 },
                new Pet { Id = 5, Name = "Hamchi",  Species = "Hamster", Age = 1, Price = 500000  }
            );
        }
    }
}