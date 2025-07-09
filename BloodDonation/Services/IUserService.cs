using Models;
using Models.DTOs;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    Task<IEnumerable<User>> SearchUsersByNameAsync(string searchTerm);
    Task<(bool success, string message)> UpdateUserAsync(Guid userId, UserUpdateDTO updateDto);
    Task<User> GetUserDetailAsync(Guid id);
    Task<User> GetCurrentUserAsync(Guid userId);
    Task<IEnumerable<User>> GetUsersByFullNameAsync(string fullName);
    Task<User> GetByUserIdCardAsync(string userIdCard);
}