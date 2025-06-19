using Models;

namespace Repositories.Interfaces
{
    public interface IBloodTypeRepository
    {
        Task<IEnumerable<BloodType>> GetAllAsync();
        Task<BloodType> GetByIdAsync(Guid id);
        Task<int> GetAvailableUnitsCountAsync(Guid bloodTypeId);
        Task<int> GetTotalUnitsCountAsync(Guid bloodTypeId);
    }
}