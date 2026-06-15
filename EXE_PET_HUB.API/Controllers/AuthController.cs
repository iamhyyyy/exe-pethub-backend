using EXE_PET_HUB.Application.DTOs.Auth;
using EXE_PET_HUB.Application.Interfaces;
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

            // Phân biệt rõ lý do thất bại cần kiểm tra thêm từ service
            if (user == null)
                return Unauthorized(new { message = "Wrong email/password, or account not confirmed yet." });

            return Ok(user);
        }
        // Logout ở REST API: client tự xóa token ở phía mình
        // Nếu muốn có endpoint cho rõ ràng:
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Không cần xử lý gì phía server với JWT
            // Client xóa token khỏi localStorage/cookie là xong
            return Ok(new { message = "Logout Successfully!" });
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
        public async Task<IActionResult> RegisterManager([FromBody] RegisterManagerRequest request)
        {
            var (success, message) = await _authService.RegisterManagerAsync(request);
            if (!success)
                return BadRequest(new { message });
            return Ok(new { message });
        }


        // User click link trong email → gọi endpoint này
        // Link dạng: GET /api/auth/confirm-email?userId=xxx&token=yyy
        [HttpGet("confirm-email")]
        [EnableRateLimiting("OtpPolicy")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Invalid confirmation link." });

            var (success, message) = await _authService.ConfirmEmailAsync(userId, token);

            if (!success)
                return BadRequest(new { message });

            // Trả về trang thông báo thành công (hoặc redirect về frontend)
            return Ok(new { message });
        }
    }
}
