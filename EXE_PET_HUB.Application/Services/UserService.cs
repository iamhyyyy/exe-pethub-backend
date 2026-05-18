using AutoMapper;
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EXE_PET_HUB.Application.Services
{
    public class UserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            var dtoUsers = _mapper.Map<List<UserDto>>(users);

            foreach (var user in users)
            {
                foreach (var dtoUser in dtoUsers)
                {
                    if (dtoUser.Id == user.Id)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        dtoUser.Role = roles.FirstOrDefault() ?? "Customer";
                    }
                }
            }

            return dtoUsers;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) return null;

            var dto = _mapper.Map<UserDto>(user);

            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "Customer";
    
            return dto;
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