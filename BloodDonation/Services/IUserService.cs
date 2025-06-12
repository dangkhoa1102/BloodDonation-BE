using Models;

namespace Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<IEnumerable<User>> GetUsersByRoleAsync(string role);
        Task<IEnumerable<User>> SearchUsersByNameAsync(string searchTerm);
    }
}