using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs.UserDTOs
{
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
} 