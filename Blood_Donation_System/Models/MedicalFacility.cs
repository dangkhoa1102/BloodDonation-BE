using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class MedicalFacility
    {
        [Key]
        public Guid FacilityId { get; set; }
        
        [MaxLength(255)]
        public string FacilityName { get; set; }
        
        [MaxLength(500)]
        public string Address { get; set; }
        
        [MaxLength(20)]
        public string Phone { get; set; }
        
        [MaxLength(255)]
        public string Email { get; set; }
        
        public int? Capacity { get; set; }
        
        [MaxLength(255)]
        public string Specialization { get; set; }
        
        [MaxLength(100)]
        public string Coordinates { get; set; }
        
        public Guid? ClosestDonorId { get; set; }
        
        [ForeignKey("ClosestDonorId")]
        public Donor ClosestDonor { get; set; }
    }
} 