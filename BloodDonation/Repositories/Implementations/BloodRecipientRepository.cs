using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BloodRecipientRepository : BaseRepository<BloodRecipient>, IBloodRecipientRepository
    {
        public BloodRecipientRepository(BloodDonationSupportContext context) : base(context)
        {
        }

        public async Task<BloodRecipient> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.UserId == userId);
        }

        public async Task<IEnumerable<BloodRecipient>> GetByUrgencyLevelAsync(string urgencyLevel)
        {
            return await _dbSet
                .Include(r => r.User)
                .Where(r => r.UrgencyLevel == urgencyLevel)
                .ToListAsync();
        }
    }
}