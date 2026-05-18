using EXE_PET_HUB.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace EXE_PET_HUB.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public PlanType Plan { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Role { get; set; }
    }

    public class CreateUserDto
    {

    }

    public class UpdateUserDto
    {

    }
}
