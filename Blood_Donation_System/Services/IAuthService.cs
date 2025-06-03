using Blood_Donation_System.DTOs;
using Blood_Donation_System.DTOs.AuthDTOs;
using Blood_Donation_System.DTOs.UserDTOs;
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
        Task<bool> LogoutAsync(string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string token, string newPassword);
        Task<bool> ValidateResetTokenAsync(string token);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    }
} 