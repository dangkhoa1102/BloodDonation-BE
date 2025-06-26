using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enums
{
    public enum BloodDonationStatus
    {
        Pending,    // Đăng ký, đang chờ
        Approved,   // Đã duyệt, chấp nhận
        Rejected,   // Bị từ chối
        Completed,  // Đã hoàn thành
        Processing, // Đang xử lý mẫu máu
        Expired     // Đã hết hạn
    }
}
