using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("InvoiceDetail")]
    public class InvoiceDetail
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string InvoiceId { get; set; }
        public string ItemId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string ItemName { get; set; } = null!;

        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; } // Nên tính toán trước khi lưu

        [ForeignKey(nameof(InvoiceId))]
        public Invoice Invoice { get; set; } = null!;

        [ForeignKey(nameof(ItemId))]
        public Item Item { get; set; } = null!;
    }
}
