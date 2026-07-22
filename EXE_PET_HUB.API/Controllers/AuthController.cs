using EXE_PET_HUB.Application.DTOs.Auth;
using EXE_PET_HUB.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace EXE_PET_HUB.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }
        [HttpPost("login")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _authService.LoginAsync(request);
            if (user == null)
                return Unauthorized(new { message = "Sai email/password, hoặc tài khoản chưa kích hoạt." });

            return Ok(user);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Đăng Xuất Thành Công!" });
        }

        [HttpPost("register")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var (success, message) = await _authService.RegisterAsync(request);
            if (!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpPost("registerManager")]
        [EnableRateLimiting("OtpPolicy")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> RegisterManager([FromBody] RegisterManagerRequest request)
        {
            var (success, message) = await _authService.RegisterManagerAsync(request);
            if (!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpPost("join-store")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> JoinStore([FromBody] JoinStoreRequest request)
        {
            var (success, message) = await _authService.JoinStoreAsync(request);
            if (!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }

        [HttpGet("confirm-email")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Liên Kết Không Hợp Lệ." });

            var (success, message) = await _authService.ConfirmEmailAsync(userId, token);

            if (!success)
                return BadRequest(new { message });

            return Ok(new { message });
        }
    }
}
