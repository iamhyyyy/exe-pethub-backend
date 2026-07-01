using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    /// <summary>
    /// Controller xử lý upload ảnh lên Cloudinary.
    /// Dùng chung cho Pet, Item, User avatar.
    /// </summary>
    [ApiController]
    [Route("api/upload")]
    [Authorize]
    public class UploadController : ControllerBase
    {
        private readonly ICloudinaryService _cloudinaryService;

        public UploadController(ICloudinaryService cloudinaryService)
        {
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        /// Upload ảnh thú cưng.
        /// POST /api/upload/pet
        /// </summary>
        [HttpPost("pet")]
        public async Task<IActionResult> UploadPetImage(IFormFile file)
        {
            return await UploadImage(file, "PetHubManagement/pets");
        }

        /// <summary>
        /// Upload ảnh sản phẩm/dịch vụ (Item).
        /// POST /api/upload/item
        /// </summary>
        [HttpPost("item")]
        [Authorize(Roles = "manager")]
        public async Task<IActionResult> UploadItemImage(IFormFile file)
        {
            return await UploadImage(file, "PetHubManagement/items");
        }

        /// <summary>
        /// Upload avatar người dùng.
        /// POST /api/upload/avatar
        /// </summary>
        [HttpPost("avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file)
        {
            return await UploadImage(file, "PetHubManagement/avatars");
        }

        // ─── Helper ──────────────────────────────────────────────────────────────

        private async Task<IActionResult> UploadImage(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { success = false, message = "Vui lòng chọn file ảnh." });

            // Chỉ chấp nhận file ảnh
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                return BadRequest(new { success = false, message = "Chỉ chấp nhận file JPG, PNG, WEBP, GIF." });

            // Giới hạn 5MB
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { success = false, message = "File không được vượt quá 5MB." });

            try
            {
                var imageUrl = await _cloudinaryService.UploadImageAsync(file, folder);
                return Ok(new { success = true, imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
