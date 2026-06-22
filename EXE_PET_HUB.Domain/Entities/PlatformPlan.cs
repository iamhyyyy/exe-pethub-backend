using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("PlatformPlan")]
    public class PlatformPlan
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; } = null!;         

        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        public int DurationInDays { get; set; }             

        public bool IsActive { get; set; } = true;         

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
    }
}
