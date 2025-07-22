using Microsoft.EntityFrameworkCore;
using Models;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BloodRequestRepository : BaseRepository<BloodRequest>, IBloodRequestRepository
    {
        public BloodRequestRepository(BloodDonationSupportContext context) : base(context)
        {
        }

        public async Task<IEnumerable<BloodRequest>> GetByRecipientIdAsync(Guid recipientId)
        {
            return await _dbSet
                .Include(r => r.Recipient)
                .Include(r => r.BloodTypeRequiredNavigation)
                .Where(r => r.RecipientId == recipientId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetByBloodTypeAsync(Guid bloodTypeId)
        {
            return await _dbSet
                .Include(r => r.Recipient)
                .Where(r => r.BloodTypeRequired == bloodTypeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetByStatusAsync(string status)
        {
            return await _dbSet
                .Include(r => r.BloodTypeRequiredNavigation)
                .Include(r => r.Recipient)
                    .ThenInclude(r => r.User)
                .Include(r => r.BloodDonations)
                .Where(r => r.Status.ToLower() == status.ToLower())
                .ToListAsync();
        }

        public async Task<BloodRequest> GetByIdWithDetailsAsync(Guid requestId)
        {
            return await _dbSet
                .Include(r => r.BloodTypeRequiredNavigation)
                .Include(r => r.Recipient)
                    .ThenInclude(r => r.User)
                .Include(r => r.BloodDonations)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);
        }
        public async Task<IEnumerable<BloodRequest>> GetByIdsAsync(List<Guid> ids)
        {
            return await _dbSet
                .Include(r => r.Recipient)
                .Include(r => r.BloodTypeRequiredNavigation)
                .Where(r => ids.Contains(r.RequestId))
                .ToListAsync();
        }
        public void Update(BloodRequest request)
        {
            _dbSet.Update(request);
        }
    }
}