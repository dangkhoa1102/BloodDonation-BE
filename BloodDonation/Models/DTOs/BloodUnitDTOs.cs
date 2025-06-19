namespace Models.DTOs
{
    public class BloodUnitResponseDTO
    {
        public Guid UnitId { get; set; }
        public Guid? DonationId { get; set; }
        public Guid? BloodTypeId { get; set; }
        public Guid? ComponentType { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        
        // Additional fields for related data
        public string BloodTypeName { get; set; }
        public string ComponentName { get; set; }
        public string DonationInfo { get; set; }
    }
}