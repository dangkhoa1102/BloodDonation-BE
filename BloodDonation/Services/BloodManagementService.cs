using Models;
using Models.DTOs;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BloodManagementService : IBloodManagementService
    {
        private readonly IBloodTypeRepository _bloodTypeRepository;
        private readonly IBloodComponentRepository _bloodComponentRepository;

        public BloodManagementService(
            IBloodTypeRepository bloodTypeRepository,
            IBloodComponentRepository bloodComponentRepository)
        {
            _bloodTypeRepository = bloodTypeRepository;
            _bloodComponentRepository = bloodComponentRepository;
        }

        public async Task<IEnumerable<BloodTypeResponseDTO>> GetAllBloodTypesAsync()
        {
            var bloodTypes = await _bloodTypeRepository.GetAllAsync();
            var dtos = new List<BloodTypeResponseDTO>();

            foreach (var bt in bloodTypes)
            {
                var availableUnits = await _bloodTypeRepository.GetAvailableUnitsCountAsync(bt.BloodTypeId);
                var totalUnits = await _bloodTypeRepository.GetTotalUnitsCountAsync(bt.BloodTypeId);

                dtos.Add(new BloodTypeResponseDTO
                {
                    BloodTypeId = bt.BloodTypeId,
                    AboType = bt.AboType,
                    RhFactor = bt.RhFactor,
                    Description = bt.Description,
                    AvailableUnits = availableUnits,
                    TotalUnits = totalUnits
                });
            }

            return dtos;
        }

        public async Task<IEnumerable<BloodComponentResponseDTO>> GetAllBloodComponentsAsync()
        {
            var components = await _bloodComponentRepository.GetAllAsync();
            var dtos = new List<BloodComponentResponseDTO>();

            foreach (var c in components)
            {
                var availableUnits = await _bloodComponentRepository.GetAvailableUnitsCountAsync(c.ComponentId);
                var totalUnits = await _bloodComponentRepository.GetTotalUnitsCountAsync(c.ComponentId);

                dtos.Add(new BloodComponentResponseDTO
                {
                    ComponentId = c.ComponentId,
                    ComponentName = c.ComponentName,
                    CompatibilityRules = c.CompatibilityRules,
                    StorageRequirements = c.StorageRequirements,
                    AvailableUnits = availableUnits,
                    TotalUnits = totalUnits
                });
            }

            return dtos;
        }

        public async Task<BloodTypeResponseDTO> GetBloodTypeByIdAsync(Guid id)
        {
            var bloodType = await _bloodTypeRepository.GetByIdAsync(id);
            if (bloodType == null) return null;

            var availableUnits = await _bloodTypeRepository.GetAvailableUnitsCountAsync(id);
            var totalUnits = await _bloodTypeRepository.GetTotalUnitsCountAsync(id);

            return new BloodTypeResponseDTO
            {
                BloodTypeId = bloodType.BloodTypeId,
                AboType = bloodType.AboType,
                RhFactor = bloodType.RhFactor,
                Description = bloodType.Description,
                AvailableUnits = availableUnits,
                TotalUnits = totalUnits
            };
        }

        public async Task<BloodComponentResponseDTO> GetBloodComponentByIdAsync(Guid id)
        {
            var component = await _bloodComponentRepository.GetByIdAsync(id);
            if (component == null) return null;

            var availableUnits = await _bloodComponentRepository.GetAvailableUnitsCountAsync(id);
            var totalUnits = await _bloodComponentRepository.GetTotalUnitsCountAsync(id);

            return new BloodComponentResponseDTO
            {
                ComponentId = component.ComponentId,
                ComponentName = component.ComponentName,
                CompatibilityRules = component.CompatibilityRules,
                StorageRequirements = component.StorageRequirements,
                AvailableUnits = availableUnits,
                TotalUnits = totalUnits
            };
        }
    }
}