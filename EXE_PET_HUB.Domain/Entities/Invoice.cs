using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("Invoice")]
    public class Invoice
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string? PetId { get; set; }
        public string? AppointmentId { get; set; }
        public Guid? CustomerId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(PetId))]
        public Pet? Pet { get; set; }

        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public User? Customer { get; set; }

        public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
    }
}
