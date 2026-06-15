using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StoreId { get; set; }
        // Role mặc định sẽ là "customer", không để client tự chọn
    }
    public class RegisterManagerRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StoreName { get; set; }    // ← tên cửa hàng
        public string StoreAddress { get; set; } // ← địa chỉ (bắt buộc)
        public string StorePhone { get; set; }   // ← SĐT cửa hàng (bắt buộc)
    }
}
