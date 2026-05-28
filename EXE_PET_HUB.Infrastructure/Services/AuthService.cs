using EXE_PET_HUB.Application.DTOs.Auth;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager; //cái này là ASP .NetCore có sẵn ha
        private readonly SignInManager<User> _signInManager;//cái này là ASP .NetCore có sẵn ha
        private readonly IConfiguration _configuration;//cái này là ASP .NetCore có sẵn ha
        private readonly IEmailService _emailService; 
        public AuthService(UserManager<User> userManager,
                           SignInManager<User> signInManager,
                           IConfiguration configuration,
                           IEmailService emailService)       // ← inject thêm vào đây
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            // 1. Tìm user theo email
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null) return null;

            // 2. Chặn login nếu chưa xác nhận email
            if (!user.EmailConfirmed) return null;

            // 3. Kiểm tra password
            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded) return null;

            // 4. Lấy role của user
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Guest";

            // 5. Tạo JWT token
            var token = GenerateJwtToken(user, role);
            return new LoginResponse
            {
                Token = token,
                UserName = user.UserName,
                Email = user.Email,
                Role = role,
                Expiration = DateTime.UtcNow.AddHours(2)
            };
        }
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            // 2. Kiểm tra email đã tồn tại chưa
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
                return (false, "Email already exists");
            // 3. Tạo User mới — Identity tự hash password,
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = false,  // ← Chưa xác nhận, phải click link trong mail
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            var result = await _userManager.CreateAsync(user, request.Password); /* chỗ này password thì Asp.Net yêu cầu t nhất 6 ký tự
                                                                                  Có chữ hoa, chữ thường, Có số, Có ký tự đặc biệt(@, !, #...)*/
            if (!result.Succeeded)
            {
                // Identity trả về lỗi cụ thể (password yếu, thiếu ký tự đặc biệt...)
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }
            // 4. Gán role mặc định là "customer"
            await _userManager.AddToRoleAsync(user, "customer");

            // 5. Tạo token xác nhận email từ ASP.NET Identity
            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(confirmToken); // Encode vì token có ký tự đặc biệt

            // 6. Tạo link xác nhận — trỏ về API endpoint confirm-email
            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var confirmLink = $"{baseUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            // 7. Gửi mail chứa link xác nhận
            var emailSent = true;
            try
            {
                var subject = "[PetHub] Confirm your account 🐾";
                var body = $"""
                    <h2>Hello {user.UserName}!</h2>
                    <p>Your account has been successfully created on <strong>PetHub</strong>.</p>
                    <p>Now you can login and start managing your pets.</p>
                    <br/>
                    <a href="{confirmLink}" 
                       style="background:#4CAF50;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;font-weight:bold;">
                         Confirm your account
                    </a>
                    <br/><br/>
                    <p><small>Link will expire in 24 hours. If you did not register, please ignore this email.</small></p>
                    <p>Sincerely,<br/>PetHub Team 🐶🐱</p>
                    """;

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception)
            {
                emailSent = false;
            }

            var message = emailSent
                ? "Register successfully! Please check your email to confirm your account."
                : "Register successfully! (Confirmation email could not be sent, please contact support)";

            return (true, message);
        }
        public async Task<(bool Success, string Message)> ConfirmEmailAsync(string userId, string token)
        {
            // 1. Tìm user theo Id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "Invalid confirmation link.");

            // 2. Đã xác nhận rồi thì không cần làm gì thêm
            if (user.EmailConfirmed)
                return (true, "Email already confirmed. You can login now.");

            // 3. Decode token (vì khi gửi đã UrlEncode)
            var decodedToken = WebUtility.UrlDecode(token);

            // 4. Gọi Identity để xác nhận — tự động set EmailConfirmed = true trong DB
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Email confirmation failed: {errors}");
            }

            return (true, "Email confirmed successfully! You can now login.");
        }

        private string GenerateJwtToken(User user, string role)
        {
            var jwtKey = _configuration["Jwt:Key"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            // Claims = thông tin nhúng vào trong token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name,           user.UserName),
                new Claim(ClaimTypes.Email,          user.Email),
                new Claim(ClaimTypes.Role,           role)
            };
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
