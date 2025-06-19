using Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class ComponentCompatibilityDTO
    {
        public string ComponentName { get; set; }
        public BloodComponentEnum? ComponentType { get; set; }
        public List<string> CompatibleBloodTypes { get; set; } = new List<string>();
        public string CompatibilityRules { get; set; }
    }
}
