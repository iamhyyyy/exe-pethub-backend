using EXE_PET_HUB.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);                         // đăng nhập
        Task<(bool Success, string Message)> RegisterAsync(RegisterRequest request);
        Task<(bool Success, string Message)> RegisterManagerAsync(RegisterManagerRequest request);// đăng ký
        Task<(bool Success, string Message)> ConfirmEmailAsync(string userId, string token); // xác nhận email
        Task<(bool Success, string Message)> JoinStoreAsync(JoinStoreRequest request);       // user cũ tham gia thêm store
    }
}
