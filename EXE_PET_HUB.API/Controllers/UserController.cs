using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICloudinaryService _cloudinaryService;

        public UserController(IUserService userService, ICloudinaryService cloudinaryService)
        {
            _userService = userService;
            _cloudinaryService = cloudinaryService;
        }

        [HttpGet("users")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Cập nhật profile. Gửi multipart/form-data gồm các field + file avatar (tùy chọn).
        /// </summary>
        [HttpPut("user/{id}")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(Guid id, [FromForm] UpdateUserDto dto, IFormFile? file)
        {
            string? imageUrl = null;
            if (file != null)
                imageUrl = await _cloudinaryService.UploadImageAsync(file, "PetHubManagement/avatars");

            var result = await _userService.UpdateAsync(id, dto, imageUrl);
            return Ok(result);
        }

        [HttpGet("users/store")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<UserDto>>> GetCustomersByStore()
        {
            var storeId = User.Claims.FirstOrDefault(c => c.Type == "StoreId")?.Value;
            if (string.IsNullOrEmpty(storeId))
                return Unauthorized(new { message = "StoreId not found in token." });

            var users = await _userService.GetAllCustomersByStoreAsync(storeId);
            return Ok(users);
        }
    }
}
