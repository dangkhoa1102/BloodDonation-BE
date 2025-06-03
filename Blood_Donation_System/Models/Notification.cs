using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blood_Donation_System.Models
{
    public class Notification
    {
        [Key]
        public Guid NotificationId { get; set; }
        
        public Guid? UserId { get; set; }
        
        [ForeignKey("UserId")]
        public User User { get; set; }
        
        public string Message { get; set; }
        
        [MaxLength(50)]
        public string NotificationType { get; set; }
        
        public DateTime? SendDate { get; set; }
        
        public bool IsRead { get; set; } = false;
        
        public DateTime? ScheduledDate { get; set; }
    }
} 