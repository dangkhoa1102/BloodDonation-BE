using Models;

namespace Repositories.Interfaces
{
    public interface IBloodUnitRepository : IBaseRepository<BloodUnit>
    {
        Task<BloodUnit> GetByIdWithDetailsAsync(Guid id);
        Task<IEnumerable<BloodUnit>> GetAllWithDetailsAsync();
        Task<IEnumerable<BloodUnit>> GetByBloodTypeAsync(Guid bloodTypeId);
        Task<IEnumerable<BloodUnit>> GetByComponentTypeAsync(Guid componentType);
        Task<IEnumerable<BloodUnit>> GetByStatusAsync(string status);
        Task<IEnumerable<BloodUnit>> GetExpiredUnitsAsync();
    }
}