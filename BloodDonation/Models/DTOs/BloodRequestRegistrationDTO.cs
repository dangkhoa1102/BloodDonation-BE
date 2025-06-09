using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.DTOs
{
    public class BloodRequestRegistrationDTO
    {
        [Required(ErrorMessage = "Customer ID is required")]
        public Guid PatientUserId { get; set; }  // Đổi tên để rõ ràng hơn

        [Required(ErrorMessage = "Blood type is required")]
        public Guid BloodTypeRequired { get; set; }

        [Required(ErrorMessage = "Quantity needed is required")]
        [Range(1, 5000, ErrorMessage = "Quantity must be between 1 and 5000 ml")]
        public int QuantityNeeded { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }
    }
}