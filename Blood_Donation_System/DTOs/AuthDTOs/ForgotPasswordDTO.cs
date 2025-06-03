using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs.AuthDTOs
{
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
} 