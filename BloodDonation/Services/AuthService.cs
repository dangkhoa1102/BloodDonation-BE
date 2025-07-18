﻿using System;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Models;
using Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using Models.Enums;
using Repositories.Interfaces;

namespace Services
{
    public interface IAuthService
    {
        Task<(bool success, string message, string token)> LoginAsync(LoginDTO loginDto);
        Task<(bool success, string message)> RegisterAsync(RegisterDTO registerDto);
        Task<(bool success, string message)> LogoutAsync(string token);
        Task<(bool success, string message, string token)> LoginWithGoogleAsync(string email, string name);
        Task<User> GetUserByEmailAsync(string email);

    }

    public class AuthService : IAuthService
    {
        private readonly BloodDonationSupportContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;
        private static readonly HashSet<string> _blacklistedTokens = new HashSet<string>();

        public AuthService(
            BloodDonationSupportContext context,
            IConfiguration configuration,
            ILogger<AuthService> logger,
            IUserRepository userRepository,
            IJwtService jwtService)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _userRepository = userRepository;
            _jwtService = jwtService;
        }

        public async Task<(bool success, string message, string token)> LoginWithGoogleAsync(string email, string name)
        {
            // 1. Tìm user theo email
            var user = await _userRepository.GetByEmailAsync(email);

            // 2. Nếu chưa có user, tạo mới
            if (user == null)
            {
                user = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = email,
                    FullName = name,
                    Username = email, // hoặc sinh username từ email/name
                    Role = UserRoles.Member.ToString()
                };
                await _userRepository.AddAsync(user);
            }

            // 3. Tạo JWT token cho user
            var token = _jwtService.GenerateToken(user);

            return (true, "Login with Google successful", token);
        }

        public async Task<(bool success, string message, string token)> LoginAsync(LoginDTO loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());

                if (user == null)
                {
                    return (false, "Invalid email or password", null);
                }

                // So sánh mật khẩu trực tiếp, không hash
                if (loginDto.Password != user.Password)
                {
                    return (false, "Invalid email or password", null);
                }

                var token = GenerateJwtToken(user);
                return (true, "Login successful", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in login");
                return (false, "An error occurred during login", null);
            }
        }

        public async Task<(bool success, string message)> RegisterAsync(RegisterDTO registerDto)
        {
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email.ToLower() == registerDto.Email.ToLower()))
                {
                    return (false, "Email already exists");
                }

                if (await _context.Users.AnyAsync(u => u.Username.ToLower() == registerDto.Username.ToLower()))
                {
                    return (false, "Username already exists");
                }

                var user = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = registerDto.Email,
                    Username = registerDto.Username,
                    Password = registerDto.Password,
                    FullName = registerDto.FullName,
                    Phone = registerDto.Phone,
                    UserIdCard = registerDto.UserIdCard,
                    DateOfBirth = registerDto.DateOfBirth,
                    Role = UserRoles.Member.ToString()
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return (true, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in registration");
                return (false, "An error occurred during registration");
            }
        }

        public async Task<(bool success, string message)> LogoutAsync(string token)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                {
                    return (false, "Token is required");
                }

                if (IsTokenBlacklisted(token))
                {
                    return (false, "Token is already invalidated");
                }

                // Thêm token vào blacklist
                BlacklistToken(token);

                return (true, "Logged out successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in logout");
                return (false, "An error occurred during logout");
            }
        }

        public static bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.Contains(token);
        }

        public static void BlacklistToken(string token)
        {
            if (!string.IsNullOrEmpty(token))
            {
                _blacklistedTokens.Add(token);
            }
        }

        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

    }
}
