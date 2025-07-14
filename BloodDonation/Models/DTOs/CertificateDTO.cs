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
        public string CertificateNumber { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public string CertificateType { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModified { get; set; }

        // Thông tin hiển thị trên Giấy chứng nhận hiến máu
        public string FullName { get; set; } = string.Empty;
        public string UserIdCard { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string Address { get; set; } = string.Empty;
        public int? Quantity { get; set; }
        public string BloodType { get; set; } = string.Empty;
        public DateOnly? BloodDonationDate { get; set; }
    }

    public class CreateCertificateDto
    {
        public Guid DonorId { get; set; }
        public Guid DonationId { get; set; }
        public Guid StaffId { get; set; }
        public string CertificateNumber { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public string CertificateType { get; set; } = string.Empty;
    }

    public class UpdateCertificateDto
    {
        public string CertificateNumber { get; set; } = string.Empty;
        public DateOnly IssueDate { get; set; }
        public string CertificateType { get; set; } = string.Empty;
    }
}
