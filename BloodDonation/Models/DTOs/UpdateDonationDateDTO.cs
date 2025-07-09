using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class UpdateDonationDateDTO
    {
        [Required]
        public Guid DonationId { get; set; }

        [Required]
        public DateOnly DonationDate { get; set; }
    }
}
