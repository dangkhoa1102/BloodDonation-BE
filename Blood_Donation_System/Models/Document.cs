using System;
using System.ComponentModel.DataAnnotations;

namespace Blood_Donation_System.Models
{
    public class Document
    {
        [Key]
        public Guid DocumentId { get; set; }
        
        [MaxLength(500)]
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        [MaxLength(100)]
        public string Category { get; set; }
        
        public DateTime? UploadDate { get; set; }
        
        [MaxLength(50)]
        public string FileType { get; set; }
        
        public int DownloadCount { get; set; } = 0;
    }
} 