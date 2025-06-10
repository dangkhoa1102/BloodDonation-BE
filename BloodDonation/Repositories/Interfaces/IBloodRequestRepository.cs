using Models;

namespace Repositories.Interfaces
{
    public interface IBloodRequestRepository : IBaseRepository<BloodRequest>
    {
        Task<IEnumerable<BloodRequest>> GetByRecipientIdAsync(Guid recipientId);
        Task<IEnumerable<BloodRequest>> GetByBloodTypeAsync(Guid bloodTypeId);
        Task<IEnumerable<BloodRequest>> GetByStatusAsync(string status);
        Task<BloodRequest> GetByIdWithDetailsAsync(Guid requestId);
    }
}