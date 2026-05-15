using EXE_PET_HUB.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXE_PET_HUB.Application.DTOs
{
    public class AppointmentReminderDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string AppointmentId { get; set; }
        public DateTime ReminderTime { get; set; }
        public ReminderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAppointmentReminderDto
    {
        public string AppointmentId { get; set; }
        public DateTime ReminderTime { get; set; }
        public ReminderStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateAppointmentReminderDto
    {
        public string AppointmentId { get; set; }
        public DateTime ReminderTime { get; set; }
        public ReminderStatus Status { get; set; }
    }
}
