
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Infrastructure.Repositories
{
    public class PetRepository : IPetRepository
    {
        private static List<Pet> _pets = new();

        public Task<List<Pet>> GetAllAsync()
        {
            return Task.FromResult(_pets);
        }

        public Task<Pet> GetByIdAsync(int id)
        {
            var pet = _pets.FirstOrDefault(x => x.Id == id);
            return Task.FromResult(pet);
        }

        public Task AddAsync(Pet pet)
        {
            _pets.Add(pet);
            return Task.CompletedTask;
        }

        public Task DeleteAsync(int id)
        {
            var pet = _pets.FirstOrDefault(x => x.Id == id);
            if (pet != null)
                _pets.Remove(pet);

            return Task.CompletedTask;
        }
    }
}