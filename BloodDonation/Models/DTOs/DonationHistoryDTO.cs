using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class DonationHistoryCreateDto
    {
        [Required]
        public Guid DonorId { get; set; }

        [Required]
        public DateOnly DonationDate { get; set; }

        [Required]
        [Range(100, 1000, ErrorMessage = "Quantity must be between 100 and 1000 ml.")]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string HealthStatus { get; set; } = string.Empty;

        [Required]
        public DateOnly NextEligibleDate { get; set; }
        public Guid? CertificateId { get; set; }
    }

    public class DonationHistoryUpdateDto
    {
        [Required]
        public DateOnly DonationDate { get; set; }

        [Required]
        [Range(100, 1000, ErrorMessage = "Quantity must be between 100 and 1000 ml.")]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string HealthStatus { get; set; } = string.Empty;

        [Required]
        public DateOnly NextEligibleDate { get; set; }
        public Guid? CertificateId { get; set; }
    }

    // DTO trả về khi lấy thông tin lịch sử hiến máu
    public class DonationHistoryResponseDto
    {
        public Guid HistoryId { get; set; }
        public Guid DonorId { get; set; }
        public DateOnly DonationDate { get; set; }
        public int Quantity { get; set; }
        public string HealthStatus { get; set; } = string.Empty;
        public DateOnly NextEligibleDate { get; set; }
        public Guid? CertificateId { get; set; }

        // Thông tin mở rộng về người hiến máu (nếu cần)
        public string? DonorName { get; set; }
        public string? DonorBloodType { get; set; }
    }
}
