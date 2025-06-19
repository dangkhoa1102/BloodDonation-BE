using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class BloodRequestRejectDTO
    {
        [Required]
        public Guid RequestId { get; set; }

        [StringLength(500, ErrorMessage = "Rejection reason cannot exceed 500 characters")]
        public string RejectionReason { get; set; }

        public DateOnly RejectionDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    }
}
