using System.ComponentModel.DataAnnotations;

namespace EXE_PET_HUB.Application.DTOs.Auth
{
    public class JoinStoreRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string StoreId { get; set; }
    }
}
