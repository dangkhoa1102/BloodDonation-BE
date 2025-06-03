using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class Donor
    {
        [Key]
        public Guid DonorId { get; set; }
        
        public Guid? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        public Guid? BloodTypeId { get; set; }
        
        [ForeignKey("BloodTypeId")]
        public BloodType BloodType { get; set; }
        
        public float? Weight { get; set; }
        
        public float? Height { get; set; }
        
        public string MedicalHistory { get; set; }
        
        public bool? IsAvailable { get; set; }
        
        public DateTime? LastDonationDate { get; set; }
        
        public DateTime? NextEligibleDate { get; set; }
        
        public Guid? LocationId { get; set; }
        
        [ForeignKey("LocationId")]
        public Location Location { get; set; }
        
        public Guid? ClosestFacilityId { get; set; }
        
        [ForeignKey("ClosestFacilityId")]
        public MedicalFacility ClosestFacility { get; set; }
    }
} 