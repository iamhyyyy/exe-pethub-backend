using EXE_PET_HUB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace EXE_PET_HUB.Domain.Entities
{
    [Table("Item")]
    public class Item : IStoreEntity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string StoreId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public ItemType Type { get; set; }
        public int? DurationInDays { get; set; }

        [ForeignKey(nameof(StoreId))]
        public Store Store { get; set; }
    }
}
