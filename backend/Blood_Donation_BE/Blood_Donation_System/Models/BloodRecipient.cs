using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class BloodRecipient
    {
        [Key]
        public Guid RecipientId { get; set; }
        
        public Guid? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        [MaxLength(50)]
        public string UrgencyLevel { get; set; }
        
        public string MedicalCondition { get; set; }
        
        [MaxLength(500)]
        public string ContactInfo { get; set; }
    }
} 