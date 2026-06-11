using EXE_PET_HUB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Formats.Asn1.AsnWriter;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("Store")]
    public class Store
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Guid ManagerId { get; set; }

        [Column(TypeName = "varchar(255)")]
        public string Name { get; set; } = null!;

        [Column(TypeName = "varchar(255)")]
        public string Address { get; set; } = null!;

        [Column(TypeName = "varchar(15)")]
        public string Phone { get; set; }

        public string? storeImage { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.UtcNow.AddHours(7);
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow.AddHours(7);

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(ManagerId))]
        public User User { get; set; }
    }
}