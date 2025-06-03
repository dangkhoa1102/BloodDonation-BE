using Blood_Donation_System.DTOs;
using Blood_Donation_System.Models;
using System;
using System.Threading.Tasks;

namespace Blood_Donation_System.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(UserCreateDTO userCreateDto);
        Task<(User User, string Token)> LoginAsync(string email, string password);
        Task<bool> UserExistsAsync(string username, string email);
    }
} 