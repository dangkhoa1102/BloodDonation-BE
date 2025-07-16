using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTOs
{
    public class DonorDTO
    {
        public class DonorDto
        {
            public Guid DonorId { get; set; }
            public Guid? UserId { get; set; }
            public Guid? BloodTypeId { get; set; }
            public double? Weight { get; set; }
            public double? Height { get; set; }
            public string MedicalHistory { get; set; } = string.Empty;
            public bool? IsAvailable { get; set; }
            public DateOnly? LastDonationDate { get; set; }
            public DateOnly? NextEligibleDate { get; set; }
            public Guid? LocationId { get; set; }
            public Guid? ClosestFacilityId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string CurrentMedications { get; set; } = string.Empty;
        }
        public class CreateDonorDto
        {

            public Guid? UserId { get; set; }
            public Guid? BloodTypeId { get; set; }
            public double? Weight { get; set; }
            public double? Height { get; set; }
            public string MedicalHistory { get; set; } = string.Empty;
            public bool? IsAvailable { get; set; }
            public DateOnly? LastDonationDate { get; set; }
            public DateOnly? NextEligibleDate { get; set; }
            public Guid? LocationId { get; set; }
            public Guid? ClosestFacilityId { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public string Address { get; set; } = string.Empty;
            public string CurrentMedications { get; set; } = string.Empty;
        }

        public class UpdateDonorDto
        {
            public Guid? UserId { get; set; }
            public Guid? BloodTypeId { get; set; }
            public double? Weight { get; set; }
            public double? Height { get; set; }
            public string MedicalHistory { get; set; } = string.Empty;
            public bool? IsAvailable { get; set; }
            public DateOnly? LastDonationDate { get; set; }
            public DateOnly? NextEligibleDate { get; set; }
            public Guid? LocationId { get; set; }
            public Guid? ClosestFacilityId { get; set; }
        }

        public class DonorHealthCheckDto
        {
            public Guid DonorId { get; set; }
            public bool IsEligible { get; set; }
            public string Message { get; set; }
            public DateOnly? NextEligibleDate { get; set; }
            public double? Weight { get; set; }
            public double? Height { get; set; }
        }


    }
}
