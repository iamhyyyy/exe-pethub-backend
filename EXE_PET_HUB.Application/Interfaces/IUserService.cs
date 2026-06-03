using EXE_PET_HUB.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllAsync();
        Task<UserDto?> GetByIdAsync(Guid id);
        Task<ResponeUserDto> UpdateAsync(UpdateUserDto dto);
    }
}
