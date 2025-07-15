using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class BloodQuantityStatsDTO
    {
        public Dictionary<string, int> BloodTypeQuantities { get; set; } = new();
        public int TotalQuantity { get; set; }
    }
}
