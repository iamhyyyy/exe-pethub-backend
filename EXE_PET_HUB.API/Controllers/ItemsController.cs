using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemService _itemService;
        public ItemsController(ItemService itemService)
        {
            _itemService = itemService;
        }
        [HttpGet("item")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<ItemDto>>> GetAll()
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var items = await _itemService.GetAllAsync(id);
            return Ok(items);
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ItemDto>> GetById(string id)
        {
            var Storeid = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var item = await _itemService.GetByIdAsync(Storeid, id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        [HttpPost]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<ItemDto>> Create(CreateItemDto dto)
        {
            var id = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var createdItem = await _itemService.CreateAsync(id, dto);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult> Update(string id, UpdateItemDto dto)
        {
            var Storeid = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var updated = await _itemService.UpdateAsync(Storeid, id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
