using Blood_Donation_System.DTOs;
using Blood_Donation_System.DTOs.AuthDTOs;
using Blood_Donation_System.DTOs.UserDTOs;
using Blood_Donation_System.Models;
using Blood_Donation_System.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace Blood_Donation_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(
            IUserRepository userRepository, 
            IJwtService jwtService,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<User> RegisterAsync(UserCreateDTO userCreateDto)
        {
            if (await UserExistsAsync(userCreateDto.Username, userCreateDto.Email))
            {
                return null;
            }

            var user = new User
            {
                UserId = Guid.NewGuid(),
                Username = userCreateDto.Username,
                Password = userCreateDto.Password, // Trong thực tế, bạn nên mã hóa mật khẩu
                UserIdCard = userCreateDto.UserIdCard,
                Email = userCreateDto.Email,
                FullName = userCreateDto.FullName,
                Phone = userCreateDto.Phone,
                DateOfBirth = userCreateDto.DateOfBirth,
                Role = Role.Customer // Mặc định role là Customer
            };

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return user;
        }

        public async Task<(User User, string Token)> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetUserByEmail(email);
            
            // Trong thực tế, bạn nên sử dụng phương pháp so sánh mật khẩu an toàn
            if (user != null && user.Password == password)
            {
                var token = _jwtService.GenerateToken(user);
                return (user, token);
            }

            return (null, null);
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            var existingUserByUsername = await _userRepository.GetUserByUsername(username);
            var existingUserByEmail = await _userRepository.GetUserByEmail(email);
            
            return existingUserByUsername != null || existingUserByEmail != null;
        }

        public async Task<bool> LogoutAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;

            // Thêm token vào blacklist hoặc revoke token
            await _jwtService.RevokeTokenAsync(token);
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetUserByEmail(email);
            if (user == null)
                return false;

            // Tạo reset token
            var resetToken = GenerateResetToken();
            user.ResetPasswordToken = resetToken;
            user.ResetPasswordTokenExpiry = DateTime.UtcNow.AddHours(24);

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Gửi email với reset token
            var resetLink = $"{_configuration["AppUrl"]}/reset-password?token={resetToken}";
            var emailBody = $"Để đặt lại mật khẩu của bạn, vui lòng nhấp vào liên kết sau: {resetLink}";
            
            await _emailService.SendEmailAsync(
                email,
                "Đặt lại mật khẩu",
                emailBody
            );

            return true;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var user = await _userRepository.GetUserByResetToken(token);
            if (user == null || !IsResetTokenValid(user))
                return false;

            // Cập nhật mật khẩu mới
            user.Password = HashPassword(newPassword);
            user.ResetPasswordToken = null;
            user.ResetPasswordTokenExpiry = null;

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ValidateResetTokenAsync(string token)
        {
            var user = await _userRepository.GetUserByResetToken(token);
            return user != null && IsResetTokenValid(user);
        }

        private string GenerateResetToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private bool IsResetTokenValid(User user)
        {
            return user.ResetPasswordTokenExpiry.HasValue &&
                   user.ResetPasswordTokenExpiry.Value > DateTime.UtcNow;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Kiểm tra mật khẩu hiện tại
            if (user.Password != currentPassword) // Trong thực tế, bạn nên sử dụng phương pháp so sánh mật khẩu an toàn
                return false;

            // Cập nhật mật khẩu mới
            user.Password = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }
    }
} 