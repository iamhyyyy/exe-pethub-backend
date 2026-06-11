using EXE_PET_HUB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Formats.Asn1.AsnWriter;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("StoreCustomer")]
    public class StoreCustomer
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StoreId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow.AddHours(7);

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public User Customer { get; set; } = null!;
    }
}