using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace EXE_PET_HUB.Infrastructure.Identity
{
    public class AppUser : IdentityUser
    {
        public string FullName { get; set; }
    }
}
