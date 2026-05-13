using EXE_PET_HUB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.DTOs
{
    
    // DTO để trả dữ liệu cho Frontend (có Id)
    public class ItemDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public ItemType Type { get; set; }
    }
    // DTO để Frontend gửi lên khi tạo mới (không cần Id)
    public class CreateItemDto
    {
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public ItemType Type { get; set; }
    }
    // DTO để Frontend gửi lên khi cập nhật
    public class UpdateItemDto
    {
        public string Name { get; set; } = null!;
        public double Price { get; set; }
        public ItemType Type { get; set; }
    }

}
