using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetRepository _petRepository;

        public PetsController(IPetRepository petRepository)
        {
            _petRepository = petRepository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Pet>>> GetAll()
        {
            var pets = await _petRepository.GetAllAsync();
            return Ok(pets);
        }
    }
}