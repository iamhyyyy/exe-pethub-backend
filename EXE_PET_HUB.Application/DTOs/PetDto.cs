namespace EXE_PET_HUB.Application.DTOs
{
    public class PetDto
    {
        public string? Id { get; set; }
        public string StoreId { get; set; }
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Color { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class CreatePetDto
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Color { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class UpdatePetDto
    {
        public Guid CustomerId { get; set; }
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string? Color { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? ImageUrl { get; set; }
    }
}
