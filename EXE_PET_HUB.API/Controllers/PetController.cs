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
        private readonly ICloudinaryService _cloudinaryService;

        public PetController(PetService petService, ICloudinaryService cloudinaryService)
        {
            _petService = petService;
            _cloudinaryService = cloudinaryService;
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

        /// <summary>
        /// Tạo pet mới. Gửi multipart/form-data gồm các field + file ảnh (tùy chọn).
        /// </summary>
        [HttpPost("pet")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<PetDto>> Create([FromForm] CreatePetDto dto, IFormFile? file)
        {
            string? imageUrl = null;
            if (file != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(file, "PetHubManagement/pets");

            var pet = await _petService.CreateAsync(dto, imageUrl);
            return CreatedAtAction(nameof(GetById), new { id = pet.Id }, pet);
        }

        /// <summary>
        /// Cập nhật pet. Gửi multipart/form-data gồm các field + file ảnh mới (tùy chọn).
        /// </summary>
        [HttpPatch("pet/{id}")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(string id, [FromForm] UpdatePetDto dto, IFormFile? file)
        {
            string? imageUrl = null;
            if (file != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(file, "PetHubManagement/pets");

            var result = await _petService.Update(id, dto, imageUrl);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}