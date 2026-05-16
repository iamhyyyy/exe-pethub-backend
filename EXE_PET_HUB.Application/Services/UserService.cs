using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.DTOs;
using AutoMapper;

namespace EXE_PET_HUB.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            return _mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);

            return user == null ? null : _mapper.Map<UserDto>(user);
        }

        //public async Task<PetDto> CreateAsync(CreatePetDto dto)
        //{
        //    var pet = _mapper.Map<Pet>(dto);
        //    pet.Id = Guid.NewGuid().ToString();
        //    await _unitOfWork.PetRepository.AddAsync(pet);
        //    await _unitOfWork.CompleteAsync();
            
        //    return _mapper.Map<PetDto>(pet);
        //}

        //public async Task<bool> Update(string id, UpdatePetDto dto)
        //{
        //    var pet = await _unitOfWork.PetRepository.GetByIdAsync(id);
        //    if (pet == null) return false;
            
        //    _mapper.Map(dto, pet);

        //    _unitOfWork.PetRepository.Update(pet);
        //    await _unitOfWork.CompleteAsync();
        //    return true;
        //}
    }
}