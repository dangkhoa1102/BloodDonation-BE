using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class BloodRequestUpdateDTO
    {
        public Guid? BloodTypeRequired { get; set; }

        [Range(1, 5000, ErrorMessage = "Quantity must be between 1 and 5000 ml")]
        public int? QuantityNeeded { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}