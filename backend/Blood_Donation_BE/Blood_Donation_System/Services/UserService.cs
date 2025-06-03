using Blood_Donation_System.Models;
using Blood_Donation_System.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blood_Donation_System.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetUserByUsername(username);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmail(email);
        }

        public async Task<bool> AddUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            
            if (await UserExistsAsync(user.Username, user.Email))
                return false;
            
            user.UserId = Guid.NewGuid();
            await _userRepository.AddAsync(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            
            _userRepository.Update(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return false;
            
            _userRepository.Remove(user);
            return await _userRepository.SaveChangesAsync();
        }

        public async Task<bool> UserExistsAsync(string username, string email)
        {
            var existingUserByUsername = await _userRepository.GetUserByUsername(username);
            var existingUserByEmail = await _userRepository.GetUserByEmail(email);
            
            return existingUserByUsername != null || existingUserByEmail != null;
        }
    }
} 