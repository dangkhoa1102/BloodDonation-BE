using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class BloodDonationProcessDTO
    {
        public Guid DonationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateOnly? HealthCheckDate { get; set; }
        public string HealthCheckStatus { get; set; } = string.Empty;
        public Guid? CertificateId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public Guid? DonorId { get; set; }
    }
}
