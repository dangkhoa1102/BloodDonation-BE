using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class DonationHistory
    {
        [Key]
        public Guid HistoryId { get; set; }
        
        public Guid? DonorId { get; set; }
        
        [ForeignKey("DonorId")]
        public Donor Donor { get; set; }
        
        public DateTime? DonationDate { get; set; }
        
        public int? Quantity { get; set; }
        
        public string HealthStatus { get; set; }
        
        public DateTime? NextEligibleDate { get; set; }
    }
} 