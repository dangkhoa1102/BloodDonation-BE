using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;
using Models;

namespace Repositories.Implementations
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(BloodDonationSupportContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalizedEmail = email.Trim().ToLower();
            return await _dbSet.FirstOrDefaultAsync(u => u.Email.Trim().ToLower() == normalizedEmail);
        }


        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await _dbSet.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> IsUsernameExistsAsync(string username)
        {
            return await _dbSet.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet.Where(u => u.Role == role).ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersByFullNameAsync(string fullName)
        {
            return await _dbSet
                .Where(u => u.FullName.ToLower().Contains(fullName.ToLower()))
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }
        public async Task<User> GetByUserIdCardAsync(string userIdCard)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.UserIdCard.Equals(userIdCard));
        }
    }
}
