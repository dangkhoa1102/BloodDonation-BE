using System;

namespace Blood_Donation_System.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
    }
} 