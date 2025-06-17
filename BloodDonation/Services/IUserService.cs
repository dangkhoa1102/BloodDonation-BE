using Models;
using Models.DTOs;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
    Task<IEnumerable<User>> SearchUsersByNameAsync(string name);
    Task<(bool success, string message)> UpdateUserAsync(Guid userId, UserUpdateDTO updateDto);
    Task<User> GetUserDetailAsync(Guid id);
}