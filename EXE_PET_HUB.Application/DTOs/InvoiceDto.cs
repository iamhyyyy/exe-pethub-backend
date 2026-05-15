using EXE_PET_HUB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.DTOs
{
    public class InvoiceDto
    {
        public string Id { get; set; }
        public string? PetName { get; set; }
        public string? AppointmentNote { get; set; }
        public string? CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 
    }

    public class ResponseInvoiceOfCreateDto
    {
        public string Id { get; set; }
        public string? PetName { get; set; }
        public string? AppointmentNote { get; set; }
        public string? CustomerName { get; set; }
        public ICollection<InvoiceDetailsDto> Details { get; set; } = new List<InvoiceDetailsDto>();
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }

    public class InvoiceDetailsDto
    {
        public string Id { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string ItemName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public double Subtotal { get; set; }

    }

    public class CreateInvoiceDetailDto
    {
        public string ItemName { get; set; } = null!;
        public decimal Price { get; set; }
        public int Quantity { get; set; }

    }

    public class CreateInvoiceDto
    {
        public string? PetId { get; set; }
        public string? AppointmentId { get; set; }
        public Guid? CustomerId { get; set; }
        public ICollection<CreateInvoiceDetailDto> Details { get; set; } = new List<CreateInvoiceDetailDto>();

    }
}
