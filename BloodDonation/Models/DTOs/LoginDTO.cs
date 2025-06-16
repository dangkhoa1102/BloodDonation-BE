using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class LoginDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email ")]
        [DefaultValue("example@gmail.com")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
} 