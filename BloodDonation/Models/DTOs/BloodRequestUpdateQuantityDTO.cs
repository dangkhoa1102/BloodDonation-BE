using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class BloodRequestUpdateQuantityDTO
    {
        public Guid RequestId { get; set; }
        public int ReceivedQuantity { get; set; }
    }
}
