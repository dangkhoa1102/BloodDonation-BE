﻿using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.Enums;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BloodUnitService : IBloodUnitService
    {
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly ILogger<BloodUnitService> _logger;

        public BloodUnitService(
            IBloodUnitRepository bloodUnitRepository,
            IBloodRequestRepository bloodRequestRepository,
            ILogger<BloodUnitService> logger)
        {
            _bloodUnitRepository = bloodUnitRepository;
            _bloodRequestRepository = bloodRequestRepository;
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

                var oldRequestId = bloodUnit.RequestId;

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
                {
                    if (bloodUnit.RequestId != dto.RequestId)
                    {
                        bloodUnit.RequestId = dto.RequestId;

                        if (dto.RequestId.HasValue)
                        {
                            await UpdateBloodRequestQuantityAsync(dto.RequestId.Value, bloodUnit.Quantity);
                        }

                        if (oldRequestId.HasValue)
                        {
                            await UpdateBloodRequestQuantityAsync(oldRequestId.Value, -bloodUnit.Quantity);
                        }
                    }
                }

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

        private async Task UpdateBloodRequestQuantityAsync(Guid requestId, int quantity)
        {
            var request = await _bloodRequestRepository.GetByIdWithDetailsAsync(requestId);
            if (request == null)
                return;

            var remainingQuantity = request.QuantityNeeded - quantity;
            request.QuantityNeeded = remainingQuantity < 0 ? 0 : remainingQuantity;

            if (request.QuantityNeeded <= 0)
            {
                request.Status = BloodRequestStatus.Done.ToString();
                _logger.LogInformation("Blood request {RequestId} marked as Done - all required blood received", requestId);
            }

            _bloodRequestRepository.Update(request);
            await _bloodRequestRepository.SaveChangesAsync();

            var notification = new Notification
            {
                NotificationId = Guid.NewGuid(),
                UserId = request.Recipient?.UserId,
                NotificationType = "Blood Request Update",
                Message = request.QuantityNeeded <= 0
                    ? $"Your blood request (ID: {request.RequestId}) has been completed. All required blood has been received."
                    : $"Your blood request (ID: {request.RequestId}) has received {quantity}ml of blood. Remaining needed: {request.QuantityNeeded}ml",
                SendDate = DateOnly.FromDateTime(DateTime.Now),
                IsRead = false
            };
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

        public async Task UpdateExpiredUnitsAsync()
        {
            try
            {
                var units = await _bloodUnitRepository.GetAllWithDetailsAsync();
                var currentDate = DateOnly.FromDateTime(DateTime.Now);

                var expiredUnits = units.Where(u =>
                    u.ExpiryDate <= currentDate &&
                    u.Status.Equals(BloodUnitStatus.Available.ToString(), StringComparison.OrdinalIgnoreCase));

                foreach (var unit in expiredUnits)
                {
                    unit.Status = BloodUnitStatus.Expired.ToString();
                    await _bloodUnitRepository.UpdateAsync(unit);
                    _logger.LogInformation("Blood unit {UnitId} status updated to Expired", unit.UnitId);
                }

                await _bloodUnitRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expired blood units");
                throw;
            }
        }

        public async Task<(bool success, string message)> MarkUnitAsUsedAsync(Guid unitId, Guid? requestId = null)
        {
            try
            {
                var unit = await _bloodUnitRepository.GetByIdAsync(unitId);
                if (unit == null)
                    return (false, "Blood unit not found");

                if (!unit.Status.Equals(BloodUnitStatus.Available.ToString(), StringComparison.OrdinalIgnoreCase) &&
                    !unit.Status.Equals(BloodUnitStatus.Reserved.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    return (false, $"Cannot mark unit as used. Current status: {unit.Status}");
                }

                unit.Status = BloodUnitStatus.Used.ToString();
                if (requestId.HasValue)
                {
                    unit.RequestId = requestId;
                }

                await _bloodUnitRepository.UpdateAsync(unit);
                await _bloodUnitRepository.SaveChangesAsync();

                _logger.LogInformation("Blood unit {UnitId} marked as Used for request {RequestId}", unitId, requestId);
                return (true, "Blood unit marked as used successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking blood unit {UnitId} as used", unitId);
                return (false, $"Error marking blood unit as used: {ex.Message}");
            }
        }

        private bool ValidateStatusTransition(string currentStatus, string newStatus)
        {
            if (!Enum.TryParse<BloodUnitStatus>(currentStatus, true, out var current) ||
                !Enum.TryParse<BloodUnitStatus>(newStatus, true, out var next))
            {
                return false;
            }

            currentStatus = current.ToString();
            newStatus = next.ToString();

            return (current, next) switch
            {
                (BloodUnitStatus.Available, BloodUnitStatus.Used) => true,
                (BloodUnitStatus.Available, BloodUnitStatus.Expired) => true,
                (BloodUnitStatus.Available, BloodUnitStatus.Reserved) => true,
                (BloodUnitStatus.Available, BloodUnitStatus.Discarded) => true,
                (BloodUnitStatus.Reserved, BloodUnitStatus.Used) => true,
                (BloodUnitStatus.Reserved, BloodUnitStatus.Available) => true,
                (BloodUnitStatus.Processing, BloodUnitStatus.Available) => true,
                _ => false
            };
        }

        public async Task<(bool success, string message)> UpdateBloodUnitStatusAsync(Guid id, BloodUnitStatus newStatus)
        {
            try
            {
                var bloodUnit = await _bloodUnitRepository.GetByIdAsync(id);
                if (bloodUnit == null)
                    return (false, "Blood unit not found");

                if (!ValidateStatusTransition(bloodUnit.Status, newStatus.ToString()))
                    return (false, $"Invalid status transition from {bloodUnit.Status} to {newStatus}");

                bloodUnit.Status = newStatus.ToString();

                await _bloodUnitRepository.UpdateAsync(bloodUnit);
                await _bloodUnitRepository.SaveChangesAsync();

                _logger.LogInformation("Blood unit {Id} status updated to {Status}", id, newStatus);
                return (true, "Blood unit status updated successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating blood unit {Id} status", id);
                return (false, $"Error updating blood unit status: {ex.Message}");
            }
        }
    }
}