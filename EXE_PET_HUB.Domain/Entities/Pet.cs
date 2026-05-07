using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("Pet")]
    public class Pet
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

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
    }
}