using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs.User
{
    public class UserCreateDTO
    {
        [Required]
        [MaxLength(100)]
        [MinLength(1)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        [MinLength(1)]
        public string Password { get; set; }

        [MaxLength(20)]
        public string UserIdCard { get; set; }

        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        [MinLength(1)]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }
    }
} 