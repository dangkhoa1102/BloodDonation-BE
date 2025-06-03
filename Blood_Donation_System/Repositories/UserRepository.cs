using Blood_Donation_System.Data;
using Blood_Donation_System.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Blood_Donation_System.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetUserByEmail(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetUserByResetToken(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.ResetPasswordToken == token);
        }
    }
} 