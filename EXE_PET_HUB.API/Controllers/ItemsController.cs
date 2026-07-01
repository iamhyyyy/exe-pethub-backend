using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ItemService _itemService;
        private readonly ICloudinaryService _cloudinaryService;

        public ItemsController(ItemService itemService, ICloudinaryService cloudinaryService)
        {
            _itemService = itemService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("item")]
        [Authorize(Roles = "manager,customer")]
        public async Task<ActionResult<List<ItemDto>>> GetAll()
        {
            var storeId = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var items = await _itemService.GetAllAsync(storeId);
            return Ok(items);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ItemDto>> GetById(string id)
        {
            var storeId = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            var item = await _itemService.GetByIdAsync(storeId, id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        /// <summary>
        /// Tạo item mới. Gửi multipart/form-data gồm các field + file ảnh (tùy chọn).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "manager")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<ItemDto>> Create([FromForm] CreateItemDto dto, IFormFile? file)
        {
            var storeId = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            string? ImageUrl = null;
            if (file != null)
                 ImageUrl = await _cloudinaryService.UploadImageAsync(file, "PetHubManagement/items");
            var createdItem = await _itemService.CreateAsync(storeId, dto, ImageUrl);
            return CreatedAtAction(nameof(GetById), new { id = createdItem.Id }, createdItem);
        }

        /// <summary>
        /// Cập nhật item. Gửi multipart/form-data gồm các field + file ảnh mới (tùy chọn).
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "manager")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(string id, [FromForm] UpdateItemDto dto, IFormFile? file)
        {
            var storeId = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            string? ImageUrl = null;
            if (file != null)
                ImageUrl = await _cloudinaryService.UploadImageAsync(file, "PetHubManagement/items");

            var updated = await _itemService.UpdateAsync(storeId, id, dto, ImageUrl);
            if (!updated) return NotFound();
            return NoContent();
        }
    }
}
