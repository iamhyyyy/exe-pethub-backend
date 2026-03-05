using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetRepository _petRepository;
        private readonly IEmailService _emailService;

        public PetsController(IPetRepository petRepository, IEmailService emailService)
        {
            _petRepository = petRepository;
            _emailService = emailService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pet>>> GetAll()
        {
            var pets = await _petRepository.GetAllAsync();
            return Ok(pets);
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