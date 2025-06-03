using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class Blog
    {
        [Key]
        public Guid BlogId { get; set; }
        
        public Guid? AuthorId { get; set; }
        
        [ForeignKey("AuthorId")]
        public User Author { get; set; }
        
        [MaxLength(500)]
        public string Title { get; set; }
        
        public string Content { get; set; }
        
        public DateTime? PublishDate { get; set; }
        
        [MaxLength(100)]
        public string Category { get; set; }
        
        public int ViewCount { get; set; } = 0;
    }
} 