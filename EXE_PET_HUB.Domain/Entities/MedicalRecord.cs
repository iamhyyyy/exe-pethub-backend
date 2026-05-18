using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("MedicalRecord")]
    public class MedicalRecord
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? PetId { get; set; }
        public string AppointmentId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Diagnosis { get; set; } = null!;

        [Column(TypeName = "varchar(255)")]
        public string Treatment { get; set; } = null!;

        [Column(TypeName = "varchar(255)")]
        public string? Prescription { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string? MedicalRecordNote { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [ForeignKey(nameof(PetId))]
        public Pet? Pet { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;
    }
}
