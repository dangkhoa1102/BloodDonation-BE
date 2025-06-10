using Models;

namespace Repositories.Interfaces
{
    public interface IBloodRecipientRepository : IBaseRepository<BloodRecipient>
    {
        Task<BloodRecipient> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<BloodRecipient>> GetByUrgencyLevelAsync(string urgencyLevel);
    }
}