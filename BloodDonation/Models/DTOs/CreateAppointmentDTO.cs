using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class CreateAppointmentDTO
    {
        [Required]
        public Guid DonorId { get; set; }

        [Required]
        public DateOnly DonationDate { get; set; }

        public int? Quantity { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
