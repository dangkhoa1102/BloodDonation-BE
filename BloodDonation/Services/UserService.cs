using Microsoft.Extensions.Logging;
using Models;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                return await _userRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            try
            {
                return await _userRepository.GetUsersByRoleAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users by role: {Role}", role);
                throw;
            }
        }

        public async Task<IEnumerable<User>> SearchUsersByNameAsync(string searchTerm)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return Enumerable.Empty<User>();

                var users = await _userRepository.GetAllAsync();
                return users.Where(u =>
                    u.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    u.Username.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .Select(u => new User
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        FullName = u.FullName,
                        Phone = u.Phone,
                        UserIdCard = u.UserIdCard,
                        DateOfBirth = u.DateOfBirth,
                        Role = u.Role
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users by name term: {SearchTerm}", searchTerm);
                throw;
            }
        }
    }
}