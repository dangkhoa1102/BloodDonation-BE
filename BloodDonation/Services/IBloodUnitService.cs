using Models.DTOs;

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

        

    }
}