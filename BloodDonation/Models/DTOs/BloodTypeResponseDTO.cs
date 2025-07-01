using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class BloodTypeResponseDTO
    {
        public Guid BloodTypeId { get; set; }
        public string AboType { get; set; }
        public string RhFactor { get; set; }
        public string Description { get; set; }
        public int TotalUnits { get; set; }
        public int AvailableUnits { get; set; }
    }
}