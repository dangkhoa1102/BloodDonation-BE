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
        [Range(100, 1000)] // Ví dụ: số ml máu hợp lệ
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string HealthStatus { get; set; }

        [Required]
        public DateOnly NextEligibleDate { get; set; }
        
    }

    public class DonationHistoryUpdateDto
    {
        [Required]
        public DateOnly DonationDate { get; set; }

        [Required]
        [Range(100, 1000)]
        public int Quantity { get; set; }

        [Required]
        [MaxLength(100)]
        public string HealthStatus { get; set; }

        [Required]
        public DateOnly NextEligibleDate { get; set; }
    }

    public class DonationHistoryResponseDto
    {
        public Guid HistoryId { get; set; }
        public Guid DonorId { get; set; }
        public DateOnly DonationDate { get; set; }
        public int Quantity { get; set; }
        public string HealthStatus { get; set; }
        public DateOnly NextEligibleDate { get; set; }

        // Nếu muốn hiển thị thêm thông tin người hiến
        public string? DonorName { get; set; }
        public string? DonorBloodType { get; set; }
    }
}
