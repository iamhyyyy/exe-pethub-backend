using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Pet>> GetAllAsync()
        {
            return await _context.Pets.ToListAsync();
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _context.Pets.FindAsync(id);
        }

        public async Task AddAsync(Pet pet)
        {
            _context.Pets.Add(pet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
            }
        }
    }
}