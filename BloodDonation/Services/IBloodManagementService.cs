using Models.DTOs;

namespace Services.Interfaces
{
    public interface IBloodManagementService
    {
        Task<IEnumerable<BloodTypeResponseDTO>> GetAllBloodTypesAsync();
        Task<IEnumerable<BloodComponentResponseDTO>> GetAllBloodComponentsAsync();
        Task<BloodTypeResponseDTO> GetBloodTypeByIdAsync(Guid id);
        Task<BloodComponentResponseDTO> GetBloodComponentByIdAsync(Guid id);
    }
}