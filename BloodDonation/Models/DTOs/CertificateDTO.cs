using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class CertificateDto
    {
        public Guid CertificateId { get; set; }
        public Guid DonorId { get; set; }
        public Guid DonationId { get; set; }
        public Guid StaffId { get; set; }
        public string CertificateNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string CertificateType { get; set; }
    }

    public class CreateCertificateDto
    {
        public Guid DonorId { get; set; }
        public Guid DonationId { get; set; }
        public Guid StaffId { get; set; }
        public string CertificateNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string CertificateType { get; set; }
    }

    public class UpdateCertificateDto
    {
        public string CertificateNumber { get; set; }
        public DateOnly IssueDate { get; set; }
        public string CertificateType { get; set; }
    }
}
