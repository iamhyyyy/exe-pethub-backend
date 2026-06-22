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
        private readonly ICurrentUserService _currentUser;

        public PetService(IUnitOfWork unitOfWork, IMapper mapper, ICurrentUserService currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<List<PetDto>> GetAllAsync()
        {
            var storeId = _currentUser.GetStoreId();
            var pets = await _unitOfWork.PetRepository.GetAllAsyncByStoreId(storeId!);
            return _mapper.Map<List<PetDto>>(pets);
        }

        public async Task<PetDto?> GetByIdAsync(string id)
        {
            var storeId = _currentUser.GetStoreId();
            var pet = await _unitOfWork.PetRepository.GetByIdAsyncAndByStoreId(id, storeId!);

            return pet == null ? null : _mapper.Map<PetDto>(pet);
        }

        public async Task<List<PetDto>> GetByCustomerIdAsync(Guid customerId)
        {
            var storeId = _currentUser.GetStoreId();
            var pets = await _unitOfWork.PetRepository.GetByCustomerIdAsyncAndStoreId(customerId, storeId!);
            return _mapper.Map<List<PetDto>>(pets);
        }

        public async Task<int> CountPetByCustomerIdAsync(Guid customerId)
        {
            var storeId = _currentUser.GetStoreId();
            var pets = await _unitOfWork.PetRepository.GetByCustomerIdAsyncAndStoreId(customerId, storeId!);
            return pets.Count;
        }

        public async Task<PetDto> CreateAsync(CreatePetDto dto)
        {
            var pet = _mapper.Map<Pet>(dto);
            pet.Id = Guid.NewGuid().ToString();
            var storeId = _currentUser.GetStoreId();
            await _unitOfWork.PetRepository.AddAsyncByStoreId(storeId!, pet);
            await _unitOfWork.CompleteAsync();
            
            return _mapper.Map<PetDto>(pet);
        }

        public async Task<bool> Update(string id, UpdatePetDto dto)
        {
            var pet = await _unitOfWork.PetRepository.GetByIdAsync(id);
            if (pet == null) return false;
            
            _mapper.Map(dto, pet);
            var storeId = _currentUser.GetStoreId();
            _unitOfWork.PetRepository.UpdateByStoreId(storeId!, pet);
            await _unitOfWork.CompleteAsync();
            return true;
        }
    }
}