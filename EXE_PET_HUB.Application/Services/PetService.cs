using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;

namespace EXE_PET_HUB.Application.Services
{
    public class PetService
    {
        private readonly IPetRepository _repository;

        public PetService(IPetRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Pet>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }
    }
}