using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.Models
{
    public class BloodComponent
    {
        [Key]
        public Guid ComponentId { get; set; }
        
        [MaxLength(100)]
        public string ComponentName { get; set; }
        
        public string CompatibilityRules { get; set; }
        
        [MaxLength(500)]
        public string StorageRequirements { get; set; }
    }
} 