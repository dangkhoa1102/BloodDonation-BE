using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class RejectBloodDonationDTO
    {
        public Guid DonationId { get; set; }
        public string RejectionReason { get; set; } = string.Empty;
        public DateOnly? RejectionDate { get; set; }
    }
}
