using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.Models
{
    public class Location
    {
        [Key]
        public Guid LocationId { get; set; }
        
        public float? Latitude { get; set; }
        
        public float? Longitude { get; set; }
        
        [MaxLength(500)]
        public string Address { get; set; }
        
        [MaxLength(100)]
        public string City { get; set; }
        
        [MaxLength(100)]
        public string District { get; set; }
    }
} 