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
    [Table("AppointmentReminder")]
    public class AppointmentReminder
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AppointmentId { get; set; }
        public DateTime ReminderTime { get; set; }
        public ReminderStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;
    }
}
