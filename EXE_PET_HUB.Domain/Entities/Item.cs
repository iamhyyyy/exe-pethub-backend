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
    [Table("Item")]
    public class Item
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public ItemType Type { get; set; }
    }
}
