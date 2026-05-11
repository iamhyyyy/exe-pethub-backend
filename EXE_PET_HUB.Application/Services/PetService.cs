using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.DTOs;

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

        public async Task<Pet?> GetByIdAsync(string id)
        {
            return await _unitOfWork.PetRepository.GetByIdAsync(id);
        }

        public async Task<PetDto> CreateAsync(PetDto dto)
        {
            var pet = new Pet
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = dto.CustomerId,
                Name = dto.Name,
                Species = dto.Species,
                Color = dto.Color,
                DateOfBirth = dto.DateOfBirth
            };
            await _unitOfWork.PetRepository.AddAsync(pet);
            await _unitOfWork.CompleteAsync();
            dto.Id = pet.Id;
            return dto;
        }

        public async Task<bool> Update(PetDto dto)
        {
            var pet = await _unitOfWork.PetRepository.GetByIdAsync(dto.Id);
            if (pet == null) return false;
            
            pet.Name = dto.Name;
            pet.Color = dto.Color;
            pet.DateOfBirth = dto.DateOfBirth;
            pet.Species = dto.Species;

            _unitOfWork.PetRepository.Update(pet);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        //public async Task DeleteAsync(int id)
        //{
        //    await _unitOfWork.PetRepository.DeleteAsync(id);
        //    await _unitOfWork.CompleteAsync();
        //}
    }
}