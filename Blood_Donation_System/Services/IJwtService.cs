using Blood_Donation_System.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Blood_Donation_System.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
        IEnumerable<Claim> GetClaimsFromToken(string token);
        Guid GetUserIdFromToken(string token);
    }
} 