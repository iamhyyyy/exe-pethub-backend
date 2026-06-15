using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public string Token { get; set; }   
        public string UserName { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? StoreId { get; set; }
        public DateTime Expiration { get; set; }
    }
}
