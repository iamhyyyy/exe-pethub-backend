
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("users")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("user/{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut("user/{id}")]
        
        public async Task<IActionResult> Update(Guid id, UpdateUserDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Id mismatch");

            var result = await _userService.UpdateAsync(dto);
            return Ok(result);
        }

    }

}