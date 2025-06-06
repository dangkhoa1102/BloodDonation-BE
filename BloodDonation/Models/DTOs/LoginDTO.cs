using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Password { get; set; }
    }
} 