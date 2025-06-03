using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.DTOs.Donor
{
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