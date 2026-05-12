
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly PetService _petService;
        private readonly IEmailService _emailService;

        public PetsController(PetService petService, IEmailService emailService)
        {
            _petService = petService;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<List<PetDto>>> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PetDto>> GetById(string id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet == null) return NotFound();
            return Ok(pet);
        }

        [HttpPost]
        public async Task<ActionResult> Create(PetDto pet)
        {
            await _petService.CreateAsync(pet);
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Update(PetDto dto)
        {
            await _petService.Update(dto);
            return NoContent();
        }

        [HttpGet("test")]
        public async Task<IActionResult> SendTest()
        {
            await _emailService.SendEmailAsync(
                "huyndse184016@fpt.edu.vn",
                "Test Mail",
                "Hello from PetHub 🐶"
            );

            return Ok("Email sent");
        }
    }

}