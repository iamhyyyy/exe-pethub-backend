

namespace EXE_PET_HUB.Application.DTOs
{
    public class MedicalRecordDto
    {
        public string Id { get; set; }
        public string? PetId { get; set; }
        public string AppointmentId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string? Prescription { get; set; }
        public string? MedicalRecordNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMedicalRecordDto
    {
        public string? PetId { get; set; }
        public string AppointmentId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string? Prescription { get; set; }
        public string? MedicalRecordNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateMedicalRecordDto
    {
        public string? PetId { get; set; }
        public string AppointmentId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public string? Prescription { get; set; }
        public string? MedicalRecordNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
