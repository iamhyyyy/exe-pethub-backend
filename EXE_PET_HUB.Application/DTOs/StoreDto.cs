namespace EXE_PET_HUB.Application.DTOs
{
    public class StoreDtoForCustomer
    {
        public string? Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class StoreDto
    {
        public string? Id { get; set; }
        public Guid ManagerId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }

    public class CreateStoreDto
    {
        public Guid ManagerId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
    }

    public class UpdateStoreDto
    {
        public Guid ManagerId { get; set; }
        public string Name { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool IsActive { get; set; } = true;
    }
}