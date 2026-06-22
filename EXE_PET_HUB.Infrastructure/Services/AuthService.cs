using EXE_PET_HUB.Application.DTOs.Auth;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
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
        private readonly AppDbContext _context;
        public AuthService(UserManager<User> userManager,
                           SignInManager<User> signInManager,
                           IConfiguration configuration,
                           IEmailService emailService, AppDbContext context)       // ← inject thêm vào đây
                           
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
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

            // 5. Xác định StoreId dựa theo role
            string? storeId = null;
            if (role == "manager")
            {
                var store = await _context.Stores
                    .FirstOrDefaultAsync(s => s.ManagerId == user.Id && s.IsActive);
                if (store == null)
                    return null; // Manager chưa có Store → không cho login         
                storeId = store.Id;
            }
            else if (role == "customer")
            {
                // Customer: bắt buộc phải truyền StoreId khi login
                if (string.IsNullOrEmpty(request.StoreId))
                    return null; // Không truyền StoreId → từ chối
                var isRegistered = await _context.StoreCustomers
                    .AnyAsync(sc => sc.StoreId == request.StoreId
                                 && sc.CustomerId == user.Id);
                if (!isRegistered)
                    return null; // Customer không thuộc store này
                storeId = request.StoreId;
            }


            // 6. Tạo JWT token (có StoreId)
            var token = GenerateJwtToken(user, role, storeId);
            return new LoginResponse
            {
                Token = token,
                UserId = user.Id.ToString(),
                UserName = user.UserName,
                Email = user.Email,
                Role = role,
                StoreId = storeId,
                Expiration = DateTime.UtcNow.AddHours(2)
            };
        }
        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request)
        {
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
                return (false, "Email already exists");
            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = false,  
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            var result = await _userManager.CreateAsync(user, request.Password); 
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }
            await _userManager.AddToRoleAsync(user, "customer");

            if (string.IsNullOrEmpty(request.StoreId))
                return (false, "Mã Cửa Hàng Không Được Để Trống.");
            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == request.StoreId && s.IsActive);
            if (store == null)
                return (false, "Không Tìm Thấy Cửa Hàng Hoặc Cửa Hàng Đang Ngưng Hoạt Động.");
            var storeCustomer = new StoreCustomer
            {
                Id = Guid.NewGuid().ToString(),
                StoreId = request.StoreId,
                CustomerId = user.Id,
                CreateAt = DateTime.UtcNow.AddHours(7)
            };
            _context.StoreCustomers.Add(storeCustomer);
            await _context.SaveChangesAsync();

            var confirmToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(confirmToken);

            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var confirmLink = $"{baseUrl}/confirm-email?userId={user.Id}&token={encodedToken}";

            var emailSent = true;
            try
            {
                var subject = "[PetHub] Confirm your account 🐾";
                var body = $"""
                    <h2>Xin chào {user.UserName}!</h2>
                    <p>Tài khoản của bạn đã được tạo thành công trên <strong>PetHub</strong>.</p>
                    <p>Bây giờ bạn có thể đăng nhập và bắt đầu quản lý thú cưng của mình.</p>
                    <br/>
                    <a href="{confirmLink}" 
                       style="background:#4CAF50;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;font-weight:bold;">
                         Xác Nhận Tài Khoản
                    </a>
                    <br/><br/>
                    <p><small>Liên Kết Sẽ Hết Hạn Sau 24 Giờ. Nếu Bạn Không Đăng Ký Tài Khoản, Vui Lòng Bỏ Qua Email Này.</small></p>
                    <p>Trân Trọng,<br/>Đội Ngũ PetHub 🐶🐱</p>
                    """;

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception)
            {
                emailSent = false;
            }

            var message = emailSent
                ? "Đăng Ký Tài Khoản Thành Công! Vui Lòng Kiểm Tra Email Để Xác Nhận Tài Khoản."
                : "Đăng Ký Tài Khoản Thành Công! Vui Lòng Kiểm Tra Email Để Xác Nhận Tài Khoản.";

            return (true, message);
        }

        public async Task<(bool Success, string Message)> RegisterManagerAsync(RegisterManagerRequest request)
        {
            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
                return (false, "Email đã tồn tại");

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                EmailConfirmed = true,   
                FirstName = request.FirstName,
                LastName = request.LastName,
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            await _userManager.AddToRoleAsync(user, "manager");

            var store = new Store
            {
                Id = Guid.NewGuid().ToString(),
                ManagerId = user.Id,
                Name = request.StoreName,
                Address = request.StoreAddress,
                Phone = request.StorePhone,
                IsActive = true,
                CreateAt = DateTime.UtcNow.AddHours(7),
                UpdateAt = DateTime.UtcNow.AddHours(7),
            };
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();

            var baseUrl = _configuration["AppSettings:BaseUrl"];
            var directLink = $"{baseUrl}/auth/login";
            var emailSent = true;
            try
            {
                var subject = "[PetHub] Tài khoản Manager của bạn đã được tạo 🐾";
                var body = $"""
                    <h2>Xin chào {user.UserName}!</h2>
                    <p>Tài khoản Manager của bạn đã được tạo thành công trên <strong>PetHub</strong>.</p>
                    <hr/>
                    <p><strong>Thông tin đăng nhập:</strong></p>
                    <p>Email: {request.Email}</p>
                    <p>Password: {request.Password}</p>
                    <hr/>
                    <p><strong>Thông tin cửa hàng:</strong></p>
                    <p>Tên cửa hàng: {store.Name}</p>
                    <p>Store ID: {store.Id}</p>
                    <p>Địa chỉ: {store.Address}</p>
                    <p><em>Vui lòng đổi mật khẩu sau khi đăng nhập lần đầu.</em></p>
                    <br/>
                    <a href="{directLink}"
                       style="background:#4CAF50;color:white;padding:12px 24px;text-decoration:none;border-radius:6px;font-weight:bold;">
                      Đăng nhập ngay
                    </a>
                    <br/><br/>
                    <p>Sincerely,<br/>PetHub Team 🐶🐱</p>
                    """;

                await _emailService.SendEmailAsync(user.Email, subject, body);
            }
            catch (Exception)
            {
                emailSent = false;
            }

            var message = emailSent
                ? $"Đã tạo Manager và Store '{store.Name}' thành công! Store ID: {store.Id}"
                : $"Đã tạo Manager và Store '{store.Name}' thành công! (Gửi email thất bại). Store ID: {store.Id}";

            return (true, message);
        }
        public async Task<(bool Success, string Message)> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "Liên Kết Xác Nhận Không Hợp Lệ.");

            if (user.EmailConfirmed)
                return (true, "Email đã được xác nhận. Bạn có thể đăng nhập ngay bây giờ.");
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, $"Xác Nhận Email Thất Bại: {errors}");
            }

            return (true, "Email đã được xác nhận. Bạn có thể đăng nhập ngay bây giờ.");
        }

        public async Task<(bool Success, string Message)> JoinStoreAsync(JoinStoreRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return (false, "Tài Khoản Không Tồn Tại !");

        
            if (!user.EmailConfirmed)
                return (false, "Tài Khoản Chưa Được Xác Nhận Email!");

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!passwordCheck.Succeeded)
                return (false, "Sai Mật Khẩu!");

            var store = await _context.Stores
                .FirstOrDefaultAsync(s => s.Id == request.StoreId && s.IsActive);
            if (store == null)
                return (false, "Không Tìm Thấy Cửa Hàng!");

            var alreadyJoined = await _context.StoreCustomers
                .AnyAsync(sc => sc.StoreId == request.StoreId && sc.CustomerId == user.Id);
            if (alreadyJoined)
                return (false, "Tài Khoản Đã Tồn Tại Trong Cửa Hàng Này!");

            var storeCustomer = new StoreCustomer
            {
                Id = Guid.NewGuid().ToString(),
                StoreId = request.StoreId,
                CustomerId = user.Id,
                CreateAt = DateTime.UtcNow.AddHours(7)
            };
            _context.StoreCustomers.Add(storeCustomer);
            await _context.SaveChangesAsync();

            return (true, $"Đăng Ký Tài Khoản Vào Cửa Hàng Thành Công!");
        }

        private string GenerateJwtToken(User user, string role, string? storeId = null)
        {
            var jwtKey = _configuration["Jwt:Key"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim> 
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name,           user.UserName),
        new Claim(ClaimTypes.Email,          user.Email),
        new Claim(ClaimTypes.Role,           role)
    };
            if (!string.IsNullOrEmpty(storeId))
                claims.Add(new Claim("StoreId", storeId));
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
