using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs.User
{
    public class UserLoginDTO
    {
        [Required]
        [MinLength(1)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(1)]
        public string Password { get; set; }
    }
} 