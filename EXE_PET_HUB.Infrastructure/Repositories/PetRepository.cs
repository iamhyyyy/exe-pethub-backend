using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class PetRepository : GenericRepository<Pet>, IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context) : base(context)
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

        public Task AddAsync(Pet pet)
        {
            _context.Pets.Add(pet);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Pet pet)
        {
            _context.Pets.Update(pet);
            return Task.CompletedTask;
        }

        //public async Task DeleteAsync(string id)
        //{
        //    var pet = await _context.Pets.FindAsync(id);
        //    if (pet != null)
        //    {
        //        _context.Pets.Remove(pet);
        //    }
        //}
    }
}