using AutoMapper;
using EXE_PET_HUB.Application.DTOs;
using EXE_PET_HUB.Application.Interfaces;
using EXE_PET_HUB.Domain.Entities;
using EXE_PET_HUB.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace EXE_PET_HUB.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<User> userManager, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _unitOfWork.UserRepository.GetAllAsync();

            var dtoUsers = _mapper.Map<List<UserDto>>(users);

            foreach (var user in users)
            {
                foreach (var dtoUser in dtoUsers)
                {
                    if (dtoUser.Id == user.Id)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        dtoUser.Role = roles.FirstOrDefault() ?? "Customer";
                    }
                }
            }

            return dtoUsers;
        }

        public async Task<UserDto?> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(id);
            if (user == null) return null;

            var dto = _mapper.Map<UserDto>(user);

            var roles = await _userManager.GetRolesAsync(user);
            dto.Role = roles.FirstOrDefault() ?? "Customer";
    
            return dto;
        }
        public async Task<ResponeUserDto> UpdateAsync(UpdateUserDto dto)
        {
            var user = await _unitOfWork.UserRepository.GetByIdAsync(dto.Id);

            if (user == null)
                throw new Exception("User not found");

            // ① Lưu lại giá trị CŨ trước khi map
            var oldFirstName = user.FirstName;
            var oldLastName = user.LastName;
            var oldPlan = user.Plan;

            _mapper.Map(dto, user);
            user.UpdatedAt = DateTime.UtcNow.AddHours(7);

            _unitOfWork.Repository<User>().Update(user);
            await _unitOfWork.CompleteAsync();

            if (!string.IsNullOrEmpty(user.Email))
            {
                await SendProfileUpdatedEmailAsync(user, oldFirstName, oldLastName, oldPlan);
            }

            return _mapper.Map<ResponeUserDto>(user);
        }

        private async Task SendProfileUpdatedEmailAsync(User user,string? oldFirstName, string? oldLastName, PlanType oldPlan)
        {
            var changes = new List<string>();

            if (user.FirstName != oldFirstName)
                changes.Add($"<li><strong>First Name:</strong> {oldFirstName ?? "—"} → {user.FirstName ?? "—"}</li>");

            if (user.LastName != oldLastName)
                changes.Add($"<li><strong>Last Name:</strong> {oldLastName ?? "—"} → {user.LastName ?? "—"}</li>");

            if (user.Plan != oldPlan)
                changes.Add($"<li><strong>Plan:</strong> {oldPlan} → {user.Plan}</li>");

            // Không có gì thay đổi thì không gửi mail
            if (!changes.Any()) return;

            var changeList = string.Join("\n", changes);
            var displayName = $"{user.FirstName} {user.LastName}".Trim();
            if (string.IsNullOrWhiteSpace(displayName)) displayName = user.UserName;

            var body = $@"
            <div style='font-family: ""Segoe UI"", sans-serif; max-width: 600px; margin: auto; border: 1px solid #eee; border-radius: 15px; overflow: hidden;'>
                <div style='background: linear-gradient(135deg, #4CAF50, #2E7D32); color: white; padding: 30px; text-align: center;'>
                    <h1 style='margin: 0;'>🐾 PET HUB</h1>
                    <p style='margin: 5px 0 0; opacity: 0.9;'>Account Update Notification</p>
                </div>
                <div style='padding: 30px;'>
                    <h2 style='color: #2E7D32;'>Hello {displayName},</h2>
                    <p>Your profile has been successfully updated on <strong>{user.UpdatedAt:dd/MM/yyyy HH:mm}</strong>.</p>
                    <div style='background: #f8fbf8; padding: 20px; border-left: 4px solid #4CAF50; border-radius: 8px; margin: 20px 0;'>
                        <p><strong>Changes made:</strong></p>
                        <ul style='margin: 0; padding-left: 20px; line-height: 2;'>
                            {changeList}
                        </ul>
                    </div>
                    <p style='color: #e53935;'>⚠️ If you did not make these changes, please contact support immediately.</p>
                </div>
                <div style='background: #f4f4f4; padding: 20px; text-align: center; font-size: 12px; color: #999;'>
                    <p><strong>Pet Hub System</strong><br>Dĩ An, Bình Dương, Vietnam</p>
                </div>
            </div>";

            try
            {
                await _emailService.SendEmailAsync(
                    user.Email!,
                    "[PetHub] Your profile has been updated",
                    body);
            }
            catch
            {
                // Không để lỗi email làm fail cả luồng update
            }
        }
    }
}