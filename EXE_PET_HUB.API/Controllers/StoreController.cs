
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class StoreController : ControllerBase
    {
        private readonly StoreService _storeService;
        private readonly IEmailService _emailService;

        public StoreController(StoreService storeService, IEmailService emailService)
        {
            _storeService = storeService;
            _emailService = emailService;
        }

        [HttpGet("stores")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<StoreDto>>> GetAll()
        {
            var stores = await _storeService.GetAllAsync();
            return Ok(stores);
        }

        [HttpGet("for-customer/stores")]
        public async Task<ActionResult<List<StoreDtoForCustomer>>> GetAllStoreForCustomer()
        {
            var stores = await _storeService.GetAllForCustomerAsync();
            return Ok(stores);
        }

        [HttpGet("store/{id}")]
        public async Task<ActionResult<StoreDto>> GetById(string id)
        {
            var store = await _storeService.GetByIdAsync(id);
            if (store == null) return NotFound();
            return Ok(store);
        }

        [HttpPost("store")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<StoreDto>> Create(CreateStoreDto dto)
        {
            var store = await _storeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = store.Id }, store);
        }

        [HttpPatch("store/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Update(string id, UpdateStoreDto dto)
        {
            var store = await _storeService.Update(id, dto);
            if (!store) return NotFound();
            return NoContent();
        }

        
        [HttpDelete("store/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> Delete(string id)
        {
            var deleted = await _storeService.Delete(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }

}