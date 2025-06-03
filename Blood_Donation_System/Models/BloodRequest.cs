using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class BloodRequest
    {
        [Key]
        public Guid RequestId { get; set; }
        
        public Guid? RecipientId { get; set; }
        
        [ForeignKey("RecipientId")]
        public BloodRecipient Recipient { get; set; }
        
        public Guid? BloodTypeRequiredId { get; set; }
        
        [ForeignKey("BloodTypeRequiredId")]
        public BloodType BloodTypeRequired { get; set; }
        
        public int? QuantityNeeded { get; set; }
        
        [MaxLength(50)]
        public string UrgencyLevel { get; set; }
        
        public DateTime? RequestDate { get; set; }
        
        [MaxLength(50)]
        public string Status { get; set; }
        
        public string Description { get; set; }
    }
} 