using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EXE_PET_HUB.Application.DTOs.Auth
{
    public class LoginRequest
    {
        public string Email { get; set; }   // hoặc Email tùy team
        public string Password { get; set; }
    }
}
