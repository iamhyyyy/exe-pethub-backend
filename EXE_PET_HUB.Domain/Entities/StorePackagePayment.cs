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
    [Table("StorePackagePayment")]
    public class StorePackagePayment
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public Guid ManagerId { get; set; }

        public double Price { get; set; }
        public string PackageType { get; set; }
        public int DurationInDays { get; set; }        
        public long? PayOsOrderCode { get; set; }     
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string PaymentMethod { get; set; } = "payos";
        public string? TransactionNo { get; set; }
        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [ForeignKey(nameof(ManagerId))]
        public User Manager { get; set; } = null!;
    }
}
