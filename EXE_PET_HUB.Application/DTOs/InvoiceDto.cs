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
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? PetName { get; set; }
        public string? AppointmentNote { get; set; }
        public string? CustomerName { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 
    }

    public class InvoiceDetailsDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Column(TypeName = "varchar(255)")]
        public string ItemName { get; set; } = null!;
        public double Price { get; set; }
        public int Quantity { get; set; }
        public double Subtotal { get; set; }

    }
}
