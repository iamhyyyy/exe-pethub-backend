
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using EXE_PET_HUB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IEmailService _emailService;

        public UserController(UserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
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

        //[HttpPost("pet")]
        //public async Task<ActionResult<PetDto>> Create(CreatePetDto dto)
        //{
        //    var pet = await _petService.CreateAsync(dto);
        //    return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        //}

        //[HttpPatch("pet/{id}")]
        //public async Task<ActionResult> Update(string id, UpdatePetDto dto)
        //{
        //    var pet = await _petService.Update(id, dto);
        //    if (!pet) return NotFound();
        //    return NoContent();
        //}

    }

}