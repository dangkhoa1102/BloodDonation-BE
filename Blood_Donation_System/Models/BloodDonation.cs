using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class BloodDonation
    {
        [Key]
        public Guid DonationId { get; set; }
        
        public Guid? DonorId { get; set; }
        
        [ForeignKey("DonorId")]
        public Donor Donor { get; set; }
        
        public Guid? RequestId { get; set; }
        
        [ForeignKey("RequestId")]
        public BloodRequest Request { get; set; }
        
        public DateTime? DonationDate { get; set; }
        
        public int? Quantity { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; }
        
        public string Notes { get; set; }
    }
} 