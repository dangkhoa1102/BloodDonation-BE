using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Models.DTOs
{
    public class EmergencyBloodRequestDTO
    {
        [Required(ErrorMessage = "Tên đầy đủ là bắt buộc")]
        public string PatientName { get; set; } 

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số CMND/CCCD là bắt buộc")]
        public string UserIdCard { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Nhóm máu là bắt buộc")]
        public Guid BloodTypeRequired { get; set; }

        [Required(ErrorMessage = "Số lượng máu cần là bắt buộc")]
        [Range(1, 5000, ErrorMessage = "Số lượng máu phải từ 1-5000")]
        public int QuantityNeeded { get; set; }

        [Required(ErrorMessage = "Mô tả tình trạng bệnh nhân là bắt buộc")]
        public string Description { get; set; }
    }
}
