
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class PetController : ControllerBase
    {
        private readonly PetService _petService;

        public PetController(PetService petService)
        {
            _petService = petService;
        }

        [HttpGet("pets")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<PetDto>>> GetAll()
        {
            var pets = await _petService.GetAllAsync();
            return Ok(pets);
        }

        [HttpGet("pet/{id}")]
        [Authorize]
        public async Task<ActionResult<PetDto>> GetById(string id)
        {
            var pet = await _petService.GetByIdAsync(id);
            if (pet == null) return NotFound();
            return Ok(pet);
        }

        [HttpGet("pets/customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<List<PetDto>>> GetByCustomerId(Guid customerId)
        {
            var pets = await _petService.GetByCustomerIdAsync(customerId);
            return Ok(pets);
        }

        [HttpGet("pets/count/customer/{customerId}")]
        [Authorize]
        public async Task<ActionResult<int>> CountPetByCustomerId(Guid customerId)
        {
            var count = await _petService.CountPetByCustomerIdAsync(customerId);
            return Ok(count);
        }

        [HttpPost("pet")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<PetDto>> Create(CreatePetDto dto)
        {
            var pet = await _petService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }

        [HttpPatch("pet/{id}")]
        [Authorize]
        public async Task<ActionResult> Update(string id, UpdatePetDto dto)
        {
            var pet = await _petService.Update(id, dto);
            if (!pet) return NotFound();
            return NoContent();
        }

    }

}