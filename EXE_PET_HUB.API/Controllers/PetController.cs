
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class PetController : ControllerBase
    {
        private readonly PetService _petService;
        private readonly IEmailService _emailService;

        public PetController(PetService petService, IEmailService emailService)
        {
            _petService = petService;
            _emailService = emailService;
        }

        [HttpGet("pets")]
        public async Task<ActionResult<List<PetDto>>> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        [HttpGet("pet/{id}")]
        public async Task<ActionResult<PetDto>> GetById(string id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet == null) return NotFound();
            return Ok(pet);
        }

        [HttpGet("pets/customer/{customerId}")]
        public async Task<ActionResult<List<PetDto>>> GetByCustomerId(Guid customerId)
        {
            var pets = await _petService.GetByCustomerIdAsync(customerId);
            return Ok(pets);
        }

        [HttpPost("pet")]
        public async Task<ActionResult<PetDto>> Create(CreatePetDto dto)
        {
            var pet = await _petService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }

        [HttpPatch("pet/{id}")]
        public async Task<ActionResult> Update(string id, UpdatePetDto dto)
        {
            var pet = await _petService.Update(id, dto);
            if (!pet) return NotFound();
            return NoContent();
        }

        //[HttpGet("test")]
        //public async Task<IActionResult> SendTest()
        //{
        //    await _emailService.SendEmailAsync(
        //        "huyndse184016@fpt.edu.vn",
        //        "Test Mail",
        //        "Hello from PetHub 🐶"
        //    );

        //    return Ok("Email sent");
        //}
    }

}