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
        public async Task<(bool success, string message)> UpdateBloodUnitAsync(Guid id, UpdateBloodUnitDTO dto)
        {
            try
            {
                var bloodUnit = await _bloodUnitRepository.GetByIdAsync(id);
                if (bloodUnit == null)
                    return (false, "Blood unit not found");

                // Chỉ update các field không null
                if (dto.DonationId.HasValue)
                    bloodUnit.DonationId = dto.DonationId;

                if (dto.BloodTypeId.HasValue)
                    bloodUnit.BloodTypeId = dto.BloodTypeId;

                if (dto.ComponentType.HasValue)
                    bloodUnit.ComponentType = dto.ComponentType;

                if (dto.ExpiryDate.HasValue)
                    bloodUnit.ExpiryDate = dto.ExpiryDate;

                if (!string.IsNullOrWhiteSpace(dto.Status))
                    bloodUnit.Status = dto.Status;

                if (dto.Quantity.HasValue)
                    bloodUnit.Quantity = dto.Quantity.Value;

                if (dto.RequestId.HasValue)
                    bloodUnit.RequestId = dto.RequestId;

                await _bloodUnitRepository.UpdateAsync(bloodUnit);
                await _bloodUnitRepository.SaveChangesAsync();

                _logger.LogInformation("Blood unit {Id} updated successfully", id);
                return (true, "Blood unit updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blood unit {Id}", id);
                return (false, $"Error updating blood unit: {ex.Message}");
            }
        }
        public async Task<Dictionary<string, int>> GetQuantityByBloodTypeAsync()
        {
            try
            {
                var bloodUnits = await _bloodUnitRepository.GetAllWithDetailsAsync();

                var quantities = bloodUnits
                    .Where(bu => bu.BloodType != null && bu.Status == "available")
                    .GroupBy(bu => $"{bu.BloodType.AboType}{bu.BloodType.RhFactor}")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(bu => bu.Quantity)
                    );

                return quantities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating blood quantities by blood type");
                throw;
            }
        }

        public async Task<int> GetTotalBloodQuantityAsync()
        {
            try
            {
                var bloodUnits = await _bloodUnitRepository.GetAllWithDetailsAsync();

                var totalQuantity = bloodUnits
                    .Where(bu => bu.Status == "available")
                    .Sum(bu => bu.Quantity);

                return totalQuantity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total blood quantity");
                throw;
            }
        }
    }
}