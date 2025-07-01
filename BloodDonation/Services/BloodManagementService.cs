using Microsoft.Extensions.Logging;
using Models;
using Models.DTOs;
using Models.Enums;
using Repositories.Interfaces;
using Services.Interfaces;

namespace Services.Implementations
{
    public class BloodManagementService : IBloodManagementService
    {
        private readonly IBloodTypeRepository _bloodTypeRepository;
        private readonly IBloodComponentRepository _bloodComponentRepository;
        private readonly ILogger<BloodManagementService> _logger; // Thêm ILogger

        public BloodManagementService(
            IBloodTypeRepository bloodTypeRepository,
            IBloodComponentRepository bloodComponentRepository,
            ILogger<BloodManagementService> logger) // Thêm logger parameter
        {
            _bloodTypeRepository = bloodTypeRepository;
            _bloodComponentRepository = bloodComponentRepository;
            _logger = logger;
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

        public async Task<BloodCompatibilityDTO> GetBloodTypeCompatibilityAsync(string bloodType)
        {
            try
            {
                var bloodTypes = await _bloodTypeRepository.GetAllAsync();
                var compatibility = new BloodCompatibilityDTO { BloodType = bloodType };

                // Xác định tính tương thích dựa trên nhóm máu
                switch (bloodType.ToUpper())
                {
                    case "A+":
                        compatibility.CanDonateTo = new List<string> { "A+", "AB+" };
                        compatibility.CanReceiveFrom = new List<string> { "A+", "A-", "O+", "O-" };
                        break;
                    case "A-":
                        compatibility.CanDonateTo = new List<string> { "A+", "A-", "AB+", "AB-" };
                        compatibility.CanReceiveFrom = new List<string> { "A-", "O-" };
                        break;
                    case "B+":
                        compatibility.CanDonateTo = new List<string> { "B+", "AB+" };
                        compatibility.CanReceiveFrom = new List<string> { "B+", "B-", "O+", "O-" };
                        break;
                    case "B-":
                        compatibility.CanDonateTo = new List<string> { "B+", "B-", "AB+", "AB-" };
                        compatibility.CanReceiveFrom = new List<string> { "B-", "O-" };
                        break;
                    case "AB+":
                        compatibility.CanDonateTo = new List<string> { "AB+" };
                        compatibility.CanReceiveFrom = new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                        break;
                    case "AB-":
                        compatibility.CanDonateTo = new List<string> { "AB+", "AB-" };
                        compatibility.CanReceiveFrom = new List<string> { "A-", "B-", "AB-", "O-" };
                        break;
                    case "O+":
                        compatibility.CanDonateTo = new List<string> { "A+", "B+", "AB+", "O+" };
                        compatibility.CanReceiveFrom = new List<string> { "O+", "O-" };
                        break;
                    case "O-":
                        compatibility.CanDonateTo = new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                        compatibility.CanReceiveFrom = new List<string> { "O-" };
                        break;
                    default:
                        throw new ArgumentException("Invalid blood type");
                }

                // Lấy thông tin tương thích theo components
                var components = await _bloodComponentRepository.GetAllAsync();
                foreach (var component in components)
                {
                    // Parse componentName to enum
                    if (Enum.TryParse<BloodComponentEnum>(component.ComponentName, true, out var componentType))
                    {
                        var componentCompatibility = new ComponentCompatibilityDTO
                        {
                            ComponentName = component.ComponentName,
                            ComponentType = componentType,
                            CompatibilityRules = component.CompatibilityRules,
                            CompatibleBloodTypes = GetCompatibleBloodTypesForComponent(bloodType, componentType)
                        };
                        compatibility.ComponentCompatibility.Add(componentCompatibility);
                    }
                }

                return compatibility;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting blood type compatibility for {BloodType}", bloodType);
                throw;
            }
        }

        private string FormatComponentName(BloodComponentEnum componentType)
        {
            // Format 1: Original enum name (e.g. "RedBloodCells")
            var original = componentType.ToString();
            
            // Format 2: Display name with spaces (e.g. "Red Blood Cells") 
            var withSpaces = string.Concat(original.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
            
            // Format 3: Vietnamese name from enum comment
            var vietnameseName = componentType switch
            {
                BloodComponentEnum.WholeBlood => "Máu toàn phần",
                BloodComponentEnum.RedBloodCells => "Hồng cầu",
                BloodComponentEnum.Plasma => "Huyết tương",
                BloodComponentEnum.Platelets => "Tiểu cầu", 
                BloodComponentEnum.Cryoprecipitate => "Tủa lạnh",
                BloodComponentEnum.WhiteBloodCells => "Bạch cầu",
                _ => original
            };

            return original;
        }

        public async Task<IEnumerable<ComponentCompatibilityDTO>> GetComponentCompatibilityAsync(BloodComponentEnum componentType)
        {
            try
            {
                // Try all possible name formats
                var componentName = FormatComponentName(componentType);
                var component = await _bloodComponentRepository.GetByNameAsync(componentName);

                if (component == null)
                {
                    // If not found, get all components and try to match ignoring case
                    var allComponents = await _bloodComponentRepository.GetAllAsync();
                    component = allComponents.FirstOrDefault(c => 
                        string.Equals(c.ComponentName, componentName, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(c.ComponentName.Replace(" ", ""), componentName, StringComparison.OrdinalIgnoreCase));
                }

                if (component == null)
                {
                    _logger.LogError("Component not found in database. ComponentType: {ComponentType}, Tried name: {ComponentName}", 
                        componentType, componentName);
                    throw new ArgumentException($"Component not found for type: {componentType}");
                }

                var bloodTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };
                var compatibilities = new List<ComponentCompatibilityDTO>();

                foreach (var bloodType in bloodTypes)
                {
                    var compatibility = new ComponentCompatibilityDTO
                    {
                        ComponentName = component.ComponentName,
                        ComponentType = componentType,
                        CompatibilityRules = component.CompatibilityRules,
                        CompatibleBloodTypes = GetCompatibleBloodTypesForComponent(bloodType, componentType)
                    };
                    compatibilities.Add(compatibility);
                }

                return compatibilities;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting component compatibility for {ComponentType}", componentType);
                throw;
            }
        }

        private List<string> GetCompatibleBloodTypesForComponent(string bloodType, BloodComponentEnum componentType)
        {
            switch (componentType)
            {
                case BloodComponentEnum.WholeBlood:
                case BloodComponentEnum.RedBloodCells:
                    return bloodType.ToUpper() switch
                    {
                        "A+" => new List<string> { "A+", "A-", "O+", "O-" },
                        "A-" => new List<string> { "A-", "O-" },
                        "B+" => new List<string> { "B+", "B-", "O+", "O-" },
                        "B-" => new List<string> { "B-", "O-" },
                        "AB+" => new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" },
                        "AB-" => new List<string> { "A-", "B-", "AB-", "O-" },
                        "O+" => new List<string> { "O+", "O-" },
                        "O-" => new List<string> { "O-" },
                        _ => new List<string>()
                    };

                case BloodComponentEnum.Plasma:
                    return bloodType.ToUpper() switch
                    {
                        "A+" => new List<string> { "A+", "AB+" },
                        "A-" => new List<string> { "A+", "A-", "AB+", "AB-" },
                        "B+" => new List<string> { "B+", "AB+" },
                        "B-" => new List<string> { "B+", "B-", "AB+", "AB-" },
                        "AB+" => new List<string> { "AB+" },
                        "AB-" => new List<string> { "AB+", "AB-" },
                        "O+" => new List<string> { "A+", "B+", "AB+", "O+" },
                        "O-" => new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" },
                        _ => new List<string>()
                    };

                case BloodComponentEnum.Platelets:
                case BloodComponentEnum.Cryoprecipitate:
                case BloodComponentEnum.WhiteBloodCells:
                    // Các thành phần này thường tương thích với hầu hết các nhóm máu
                    return new List<string> { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

                default:
                    _logger.LogWarning("Unhandled component type: {ComponentType}", componentType);
                    return new List<string>();
            }
        }
    }
}