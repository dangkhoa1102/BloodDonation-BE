using Models.DTOs;
using Models.Enums;

namespace Services.Interfaces
{
    public interface IBloodUnitService
    {
        Task<IEnumerable<BloodUnitResponseDTO>> GetAllBloodUnitsAsync();
        Task<BloodUnitResponseDTO> GetBloodUnitByIdAsync(Guid id);
        Task<IEnumerable<BloodUnitResponseDTO>> GetBloodUnitsByTypeAsync(Guid bloodTypeId);
        Task<IEnumerable<BloodUnitResponseDTO>> GetBloodUnitsByComponentAsync(Guid componentId);
        Task<IEnumerable<BloodUnitResponseDTO>> GetBloodUnitsByStatusAsync(string status);
        Task<IEnumerable<BloodUnitResponseDTO>> GetExpiredBloodUnitsAsync();
        Task<(bool success, string message)> UpdateBloodUnitAsync(Guid id, UpdateBloodUnitDTO dto);
        Task<Dictionary<string, int>> GetQuantityByBloodTypeAsync();
        Task<int> GetTotalBloodQuantityAsync();
        Task UpdateExpiredUnitsAsync();
        Task<(bool success, string message)> MarkUnitAsUsedAsync(Guid unitId, Guid? requestId = null);
        Task<(bool success, string message)> UpdateBloodUnitStatusAsync(Guid id, BloodUnitStatus newStatus);
    }
}