using Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class UpdateRequestStatusDTO
    {
        [Required]
        public BloodRequestStatus NewStatus { get; set; }
    }
}