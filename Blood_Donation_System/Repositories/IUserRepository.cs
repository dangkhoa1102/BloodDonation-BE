using Blood_Donation_System.Models;
using System;
using System.Threading.Tasks;

namespace Blood_Donation_System.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetUserByUsername(string username);
        Task<User> GetUserByEmail(string email);
        Task<User> GetUserByResetToken(string token);
    }
} 