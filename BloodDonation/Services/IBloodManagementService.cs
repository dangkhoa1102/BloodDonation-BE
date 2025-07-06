using Models.DTOs;
using Models.Enums;

namespace Services.Interfaces
{
    public interface IBloodManagementService
    {
        Task<IEnumerable<BloodTypeResponseDTO>> GetAllBloodTypesAsync();
        Task<IEnumerable<BloodComponentResponseDTO>> GetAllBloodComponentsAsync();
        Task<BloodTypeResponseDTO> GetBloodTypeByIdAsync(Guid id);
        Task<BloodComponentResponseDTO> GetBloodComponentByIdAsync(Guid id);
        Task<BloodCompatibilityDTO> GetBloodTypeCompatibilityAsync(string bloodType);
        Task<IEnumerable<ComponentCompatibilityDTO>> GetComponentCompatibilityAsync(BloodComponentEnum componentType);
    }
}