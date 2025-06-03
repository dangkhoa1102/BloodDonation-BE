using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs.AuthDTOs
{
    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
} 