using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ApproveBloodDonationDTO
    {
        public Guid DonationId { get; set; }
        public DateOnly? ApproveDate { get; set; }
    }
}
