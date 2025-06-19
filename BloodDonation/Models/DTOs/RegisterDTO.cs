using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class RegisterDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [Required]
        public string UserIdCard { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
} 