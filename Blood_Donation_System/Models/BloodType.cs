using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.Models
{
    public class BloodType
    {
        [Key]
        public Guid BloodTypeId { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string AboType { get; set; }
        
        [Required]
        [MaxLength(10)]
        public string RhFactor { get; set; }
        
        [MaxLength(500)]
        public string Description { get; set; }
    }
} 