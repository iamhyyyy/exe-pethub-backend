using EXE_PET_HUB.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace EXE_PET_HUB.Application.DTOs
{
    public class AppointmentDto
    {
        public string Id { get; set; }
        public Guid CustomerId { get; set; }
        public string PetId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? AppointmentNote { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateAppointmentDto
    {
        public Guid CustomerId { get; set; }
        public string PetId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? AppointmentNote { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public Guid CustomerId { get; set; }
        public string PetId { get; set; }
        public DateOnly AppointmentDate { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string? AppointmentNote { get; set; }
        public AppointmentStatus Status { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
