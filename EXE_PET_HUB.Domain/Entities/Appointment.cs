using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EXE_PET_HUB.Domain.Enums;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("Appointment")]
    public class Appointment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Guid CustomerId { get; set; }
        public string PetId { get; set; }
        public string StoreId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? AppointmentNote { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Confirmed;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [ForeignKey(nameof(CustomerId))]
        public User Customer { get; set; } = null!;

        [ForeignKey(nameof(PetId))]
        public Pet Pet { get; set; } = null!;

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; } = null!;
    }
}
