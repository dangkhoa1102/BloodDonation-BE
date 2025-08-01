﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class HealthCheckDTO
    {
        public string UserIdCard { get; set; } = string.Empty;
        public double? Weight { get; set; }
        public double? Height { get; set; }
        public int? HeartRate { get; set; }
        public double? Temperature { get; set; }
        public string BloodPressure { get; set; } = string.Empty;
        public string MedicalHistory { get; set; } = string.Empty;
        public string CurrentMedications { get; set; } = string.Empty;
        public string Allergies { get; set; } = string.Empty;
        public DateTime HealthCheckDate { get; set; }
        public string HealthCheckStatus { get; set; } = string.Empty;
        public int? Quantity { get; set; }
    }
}
