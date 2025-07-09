using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class AppointmentDetailDTO
    {
        public Guid DonationId { get; set; }
        public Guid? DonorId { get; set; }
        public DateOnly? DonationDate { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
    }
}
