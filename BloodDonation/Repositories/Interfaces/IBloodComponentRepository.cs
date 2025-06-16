using Models;

namespace Repositories.Interfaces
{
    public interface IBloodComponentRepository
    {
        Task<IEnumerable<BloodComponent>> GetAllAsync();
        Task<BloodComponent> GetByIdAsync(Guid id);
        Task<int> GetAvailableUnitsCountAsync(Guid componentId);
        Task<int> GetTotalUnitsCountAsync(Guid componentId);
    }
}