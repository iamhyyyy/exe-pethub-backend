using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Services;
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
        public async Task<ActionResult<List<ItemDto>>> GetAll()
        {
            var items = await _itemService.GetAllAsync();
            return Ok(items);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetById(string id)
        {
            var item = await _itemService.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }
        [HttpPost]
        public async Task<ActionResult<ItemDto>> Create(CreateItemDto dto)
        {
            var createdItem = await _itemService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(string id, UpdateItemDto dto)
        {
            var updated = await _itemService.UpdateAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
