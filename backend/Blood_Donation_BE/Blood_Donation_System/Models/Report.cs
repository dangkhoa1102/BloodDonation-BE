using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class Report
    {
        [Key]
        public Guid ReportId { get; set; }
        
        [MaxLength(100)]
        public string ReportType { get; set; }
        
        public Guid? GeneratedById { get; set; }
        
        [ForeignKey("GeneratedById")]
        public User GeneratedBy { get; set; }
        
        public DateTime? GenerationDate { get; set; }
        
        public string Data { get; set; }
        
        public string Parameters { get; set; }
    }
} 