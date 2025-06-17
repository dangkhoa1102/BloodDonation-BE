using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class BloodComponentResponseDTO
    {
        public Guid ComponentId { get; set; }
        public string ComponentName { get; set; }
        public string CompatibilityRules { get; set; }
        public string StorageRequirements { get; set; }
        public int TotalUnits { get; set; }
        public int AvailableUnits { get; set; }
    }
}