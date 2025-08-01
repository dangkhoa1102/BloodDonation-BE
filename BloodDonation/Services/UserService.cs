using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Repositories.Interfaces;

namespace Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly BloodDonationSupportContext _context;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, BloodDonationSupportContext context)
        {
            _userRepository = userRepository;
            _logger = logger;
            _context = context; 
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
        public async Task<(bool success, string message)> UpdateUserAsync(Guid userId, UserUpdateDTO updateDto)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                {
                    return (false, "User not found");
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Email) && updateDto.Email != user.Email)
                {
                    var emailExists = await _userRepository.IsEmailExistsAsync(updateDto.Email);
                    if (emailExists)
                    {
                        return (false, "Email already exists");
                    }
                }


                if (!string.IsNullOrWhiteSpace(updateDto.Username) && updateDto.Username != user.Username)
                {
                    var usernameExists = await _userRepository.IsUsernameExistsAsync(updateDto.Username);
                    if (usernameExists)
                    {
                        return (false, "Username already exists");
                    }
                }
                if (!string.IsNullOrWhiteSpace(updateDto.Username))
                    user.Username = updateDto.Username;

                if (!string.IsNullOrWhiteSpace(updateDto.Email))
                    user.Email = updateDto.Email;

                if (!string.IsNullOrWhiteSpace(updateDto.FullName))
                    user.FullName = updateDto.FullName;

                if (!string.IsNullOrWhiteSpace(updateDto.Phone))
                    user.Phone = updateDto.Phone;

                if (!string.IsNullOrWhiteSpace(updateDto.UserIdCard))
                    user.UserIdCard = updateDto.UserIdCard;

                if (updateDto.DateOfBirth.HasValue)
                    user.DateOfBirth = updateDto.DateOfBirth;

                if (!string.IsNullOrWhiteSpace(updateDto.Password))
                    user.Password = updateDto.Password;

                _userRepository.UpdateAsync(user);
                await _userRepository.SaveChangesAsync();

                return (true, "User updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserId}", userId);
                return (false, "An error occurred while updating the user");
            }
        }
        public async Task<User> GetUserDetailAsync(Guid id)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(id);
                if (user == null)
                    return null;

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user details for ID: {UserId}", id);
                throw;
            }
        }
        public async Task<User> GetCurrentUserAsync(Guid userId)
        {
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return null;
                return new User
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    UserIdCard = user.UserIdCard,
                    DateOfBirth = user.DateOfBirth,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving current user with ID: {UserId}", userId);
                throw;
            }
        }
        public async Task<IEnumerable<User>> GetUsersByFullNameAsync(string fullName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fullName))
                    return Enumerable.Empty<User>();

                var users = await _userRepository.GetUsersByFullNameAsync(fullName);
                return users.Select(u => new User
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
                _logger.LogError(ex, "Error retrieving users by full name: {FullName}", fullName);
                throw;
            }
        }
        public async Task<User> GetByUserIdCardAsync(string userIdCard)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userIdCard))
                    return null;

                return await _userRepository.GetByUserIdCardAsync(userIdCard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by UserIdCard: {UserIdCard}", userIdCard);
                throw;
            }
        }
        public async Task<(bool success, string message)> DeleteUserAsync(Guid userId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    return (false, "User not found");

                // Kiểm tra xem user có phải là Admin không
                if (user.Role == "Admin")
                    return (false, "Cannot delete admin users");

                // Kiểm tra blood requests
                var hasBloodRequests = await _context.BloodRequests
                    .Include(br => br.Recipient)
                    .AnyAsync(br => br.Recipient.UserId == userId);

                if (hasBloodRequests)
                    return (false, "Cannot delete user with associated blood requests");

                // 1. Xóa notifications
                var notifications = await _context.Notifications
                    .Where(n => n.UserId == userId)
                    .ToListAsync();
                if (notifications.Any())
                {
                    _context.Notifications.RemoveRange(notifications);
                    await _context.SaveChangesAsync();
                }

                // 2. Xử lý donor và bloodDonations
                var donor = await _context.Donors
                    .Include(d => d.BloodDonations)
                    .FirstOrDefaultAsync(d => d.UserId == userId);

                if (donor != null)
                {
                    // Kiểm tra xem donor có blood units không
                    var hasBloodUnits = await _context.BloodUnits
                        .Include(bu => bu.Donation)
                        .AnyAsync(bu => bu.Donation.DonorId == donor.DonorId);

                    if (hasBloodUnits)
                    {
                        // Nếu có blood units, chỉ anonymize thông tin donor thay vì xóa
                        donor.UserId = null; // Ngắt kết nối với user
                        donor.FullName = "Deleted Donor";
                        donor.PhoneNumber = null;
                        donor.Email = null;
                        donor.Address = null;
                        donor.CurrentMedications = null;
                        _context.Donors.Update(donor);
                    }
                    else
                    {
                        // Nếu không có blood units, có thể xóa donor
                        _context.Donors.Remove(donor);
                    }
                    await _context.SaveChangesAsync();
                }

                // 3. Xóa recipient nếu có
                var recipient = await _context.BloodRecipients
                    .FirstOrDefaultAsync(r => r.UserId == userId);
                if (recipient != null)
                {
                    _context.BloodRecipients.Remove(recipient);
                    await _context.SaveChangesAsync();
                }

                // 4. Xóa user
                await _userRepository.DeleteAsync(userId);
                await _userRepository.SaveChangesAsync();

                await transaction.CommitAsync();
                return (true, "User deleted successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return (false, "An error occurred while deleting the user");
            }
        }
    }
}