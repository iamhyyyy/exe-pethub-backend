using EXE_PET_HUB.Domain.Entities;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IPetRepository
    {
        Task<List<Pet>> GetAllAsync();
        Task<Pet?> GetByIdAsync(int id);
        Task AddAsync(Pet pet);
        Task DeleteAsync(int id);
    }
}