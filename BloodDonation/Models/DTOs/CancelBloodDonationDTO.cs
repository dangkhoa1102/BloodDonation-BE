using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class CancelBloodDonationDTO
    {
        public Guid DonationId { get; set; }
        public string CancelReason { get; set; } = string.Empty;
        public DateOnly? CancelDate { get; set; }
    }
}
