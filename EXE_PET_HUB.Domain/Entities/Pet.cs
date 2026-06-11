using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Formats.Asn1.AsnWriter;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("Pet")]
    public class Pet
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StoreId { get; set; }
        public Guid CustomerId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; } = null!;

        [Column(TypeName = "varchar(255)")]
        public string Species { get; set; } = null!;

        [Column(TypeName = "varchar(255)")]
        public string? Color { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public User Customer { get; set; } = null!;

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; }
    }
}