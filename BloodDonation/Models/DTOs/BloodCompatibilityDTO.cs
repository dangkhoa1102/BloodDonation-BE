

namespace Models.DTOs
{
    public class BloodCompatibilityDTO
    {
        public string BloodType { get; set; }
        public List<string> CanDonateTo { get; set; } = new List<string>();
        public List<string> CanReceiveFrom { get; set; } = new List<string>();
        public List<ComponentCompatibilityDTO> ComponentCompatibility { get; set; } = new List<ComponentCompatibilityDTO>();
    }
}
