using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BloodUnitService : IBloodUnitService
    {
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly ILogger<BloodUnitService> _logger;

        public BloodUnitService(
            IBloodUnitRepository bloodUnitRepository,
            ILogger<BloodUnitService> logger)
        {
            _bloodUnitRepository = bloodUnitRepository;
            _logger = logger;
        }

        private BloodUnitResponseDTO MapToDTO(BloodUnit unit)
        {
            return new BloodUnitResponseDTO
            {
                UnitId = unit.UnitId,
                DonationId = unit.DonationId,
                BloodTypeId = unit.BloodTypeId,
                ComponentType = unit.ComponentType,
                ExpiryDate = unit.ExpiryDate,
                Status = unit.Status,
                Quantity = unit.Quantity,
                BloodTypeName = unit.BloodType?.AboType + unit.BloodType?.RhFactor,
                ComponentName = unit.ComponentTypeNavigation?.ComponentName,
                DonationInfo = unit.Donation != null ?
                    $"Donation Date: {unit.Donation.DonationDate}" : null
            };
        }

        public async Task<IEnumerable<BloodUnitResponseDTO>> GetAllBloodUnitsAsync()
        {
            try
            {
                var units = await _bloodUnitRepository.GetAllWithDetailsAsync();
                return units.Select(MapToDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all blood units");
                throw;
            }
        }

        public async Task<BloodUnitResponseDTO> GetBloodUnitByIdAsync(Guid id)
        {
            try
            {
                var unit = await _bloodUnitRepository.GetByIdWithDetailsAsync(id);
                return unit != null ? MapToDTO(unit) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood unit with ID: {Id}", id);
                throw;
            }
        }

        public async Task<IEnumerable<BloodUnitResponseDTO>> GetBloodUnitsByTypeAsync(Guid bloodTypeId)
        {
            try
            {
                var units = await _bloodUnitRepository.GetByBloodTypeAsync(bloodTypeId);
                return units.Select(MapToDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood units by type: {BloodTypeId}", bloodTypeId);
                throw;
            }
        }

        public async Task<IEnumerable<BloodUnitResponseDTO>> GetBloodUnitsByComponentAsync(Guid componentId)
        {
            try
            {
                var units = await _bloodUnitRepository.GetByComponentTypeAsync(componentId);
                return units.Select(MapToDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood units by component: {ComponentId}", componentId);
                throw;
            }
        }

        public async Task<IEnumerable<BloodUnitResponseDTO>> GetBloodUnitsByStatusAsync(string status)
        {
            try
            {
                var units = await _bloodUnitRepository.GetByStatusAsync(status);
                return units.Select(MapToDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blood units by status: {Status}", status);
                throw;
            }
        }

        public async Task<IEnumerable<BloodUnitResponseDTO>> GetExpiredBloodUnitsAsync()
        {
            try
            {
                var units = await _bloodUnitRepository.GetExpiredUnitsAsync();
                return units.Select(MapToDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expired blood units");
                throw;
            }
        }


    }
}