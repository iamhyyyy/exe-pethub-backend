using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.DTOs;
using AutoMapper;

namespace EXE_PET_HUB.Application.Services
{
    public class PetService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PetService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<PetDto>> GetAllAsync()
        {
            var pets = await _unitOfWork.PetRepository.GetAllAsync();

            return _mapper.Map<List<PetDto>>(pets);
        }

        public async Task<PetDto?> GetByIdAsync(string id)
        {
            var pet = await _unitOfWork.PetRepository.GetByIdAsync(id);

            return pet == null ? null : _mapper.Map<PetDto>(pet);
        }

        public async Task<PetDto> CreateAsync(CreatePetDto dto)
        {
            var pet = _mapper.Map<Pet>(dto);
            pet.Id = Guid.NewGuid().ToString();
            await _unitOfWork.PetRepository.AddAsync(pet);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<PetDto>(pet);
        }

        public async Task<bool> Update(string id, UpdatePetDto dto)
        {
            var pet = await _unitOfWork.PetRepository.GetByIdAsync(id);
            if (pet == null) return false;
            
            _mapper.Map(dto, pet);

            _unitOfWork.PetRepository.Update(pet);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}