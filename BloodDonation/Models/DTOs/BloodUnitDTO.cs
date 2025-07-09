using Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class UpdateBloodUnitDTO
    {
        public Guid? DonationId { get; set; }
        public Guid? BloodTypeId { get; set; }
        public Guid? ComponentType { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string? Status { get; set; }
        public int? Quantity { get; set; }
        public Guid? RequestId { get; set; }
    }
}