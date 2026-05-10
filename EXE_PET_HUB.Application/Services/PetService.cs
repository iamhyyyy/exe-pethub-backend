using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;

namespace EXE_PET_HUB.Application.Services
{
    public class PetService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PetService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Pet>> GetAllAsync()
        {
            return await _unitOfWork.PetRepository.GetAllAsync();
        }
    }
}