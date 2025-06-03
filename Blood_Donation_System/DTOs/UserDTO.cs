using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }
        
        [MaxLength(20)]
        public string UserIdCard { get; set; }
        
        [MaxLength(255)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; }
        
        [MaxLength(20)]
        public string Phone { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
        
        public string Role { get; set; }
    }

    public class UserCreateDTO
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
        
        [MaxLength(20)]
        public string UserIdCard { get; set; }
        
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string FullName { get; set; }
        
        [MaxLength(20)]
        public string Phone { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
    }

    public class UserUpdateDTO
    {
        [MaxLength(20)]
        public string UserIdCard { get; set; }
        
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }
        
        [MaxLength(255)]
        public string FullName { get; set; }
        
        [MaxLength(20)]
        public string Phone { get; set; }
        
        public DateTime? DateOfBirth { get; set; }
    }

    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
} 