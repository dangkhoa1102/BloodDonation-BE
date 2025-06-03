using Blood_Donation_System.DTOs;
using Blood_Donation_System.DTOs.User;
using Blood_Donation_System.Models;
using Blood_Donation_System.Repositories;
using System;
using System.Threading.Tasks;

namespace Blood_Donation_System.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtService _jwtService;

        public AuthService(IUserRepository userRepository, IJwtService jwtService)
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
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
    }
} 