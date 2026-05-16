using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Application.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public PlanType Plan { get; set; } = PlanType.Free;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool EmailConfirmed { get; set; }
    }

    public class CreateUserDto
    {

    }

    public class UpdateUserDto
    {

    }
}
