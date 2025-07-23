using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class UserDetailDTO
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserIdCard { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public int? QuantityNeeded { get; set; }
        public DateOnly? RequestDate { get; set; }
    }
}
