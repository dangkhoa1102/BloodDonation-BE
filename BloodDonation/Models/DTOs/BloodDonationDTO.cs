using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class BloodDonationDto
    {
        public Guid DonationId { get; set; }
        public Guid DonorId { get; set; }
        public Guid? RequestId { get; set; }
        public DateOnly? DonationDate { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public Guid? CertificateId { get; set; }

        // Thông tin cá nhân
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateOnly? DateOfBirth { get; set; }
        public string UserIdCard { get; set; } = string.Empty;

        // Thông tin y tế
        public string BloodType { get; set; } = string.Empty;
        public DateOnly? LastDonationDate { get; set; }
        //public string MedicalHistory { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;

        // Thông tin liên quan
        public string DonorName { get; set; } = string.Empty;
        public string RequestDescription { get; set; } = string.Empty;
    }

    public class CreateBloodDonationDto
    {
        public Guid? DonorId { get; set; }
        public Guid? RequestId { get; set; }
        public DateOnly? DonationDate { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;
        // Thêm các trường thông tin cá nhân
        public string Address { get; set; } = string.Empty;
        public string BloodType { get; set; } = string.Empty;

        // Nếu muốn cho phép nhập thủ công, nếu không sẽ tự động lấy từ Donor và User
        public bool UseDonorInfo { get; set; } = true;
    }

    public class UpdateBloodDonationDto
    {
        public DateOnly? DonationDate { get; set; }
        public int? Quantity { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;

        public Guid? CertificateId { get; set; }

        // Các trường thông tin cá nhân và y tế cũng có thể cập nhật nếu cần
        public string CurrentMedications { get; set; } = string.Empty;
    }
}
