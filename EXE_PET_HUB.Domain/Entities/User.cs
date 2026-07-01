using EXE_PET_HUB.Domain.Enums;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXE_PET_HUB.Domain.Entities
{
    // Kế thừa IdentityUser giúp bạn có sẵn Id, Email, UserName, PasswordHash, v.v.
    public class User : IdentityUser<Guid>
    {
        [Column(TypeName = "varchar(255)")]
        public string? FirstName { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? LastName { get; set; }

        public PlanType Plan { get; set; } = PlanType.Free;

        public DateTime? PremiumExpiredAt { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? ImageUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public ICollection<Pet> Pets { get; set; } = new List<Pet>();
    }
}