using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class BloodUnit
    {
        [Key]
        public Guid UnitId { get; set; }
        
        public Guid? DonationId { get; set; }
        
        [ForeignKey("DonationId")]
        public BloodDonation Donation { get; set; }
        
        public Guid? BloodTypeId { get; set; }
        
        [ForeignKey("BloodTypeId")]
        public BloodType BloodType { get; set; }
        
        public Guid? ComponentTypeId { get; set; }
        
        [ForeignKey("ComponentTypeId")]
        public BloodComponent ComponentType { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; }
    }
} 