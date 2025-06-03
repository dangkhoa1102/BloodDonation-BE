using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs
{
    public class DonorDTO
    {
        public Guid DonorId { get; set; }
        public Guid? UserId { get; set; }
        public string FullName { get; set; }
        public Guid? BloodTypeId { get; set; }
        public string BloodType { get; set; }
        public float? Weight { get; set; }
        public float? Height { get; set; }
        public string MedicalHistory { get; set; }
        public bool? IsAvailable { get; set; }
        public DateTime? LastDonationDate { get; set; }
        public DateTime? NextEligibleDate { get; set; }
        public Guid? LocationId { get; set; }
        public string Address { get; set; }
        public Guid? ClosestFacilityId { get; set; }
        public string FacilityName { get; set; }
    }

    public class DonorCreateDTO
    {
        [Required]
        public Guid UserId { get; set; }
        
        [Required]
        public Guid BloodTypeId { get; set; }
        
        [Range(0, 500)]
        public float? Weight { get; set; }
        
        [Range(0, 300)]
        public float? Height { get; set; }
        
        public string MedicalHistory { get; set; }
        
        public bool IsAvailable { get; set; } = true;
        
        public Guid? LocationId { get; set; }
        
        public Guid? ClosestFacilityId { get; set; }
    }

    public class DonorUpdateDTO
    {
        public Guid? BloodTypeId { get; set; }
        
        [Range(0, 500)]
        public float? Weight { get; set; }
        
        [Range(0, 300)]
        public float? Height { get; set; }
        
        public string MedicalHistory { get; set; }
        
        public bool? IsAvailable { get; set; }
        
        public Guid? LocationId { get; set; }
        
        public Guid? ClosestFacilityId { get; set; }
    }
} 