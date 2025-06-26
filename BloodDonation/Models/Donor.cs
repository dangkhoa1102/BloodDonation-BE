
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class Donor
{
    public Guid DonorId { get; set; }

    public Guid? UserId { get; set; }

    public Guid? BloodTypeId { get; set; }

    public bool? IsAvailable { get; set; }

    public DateOnly? LastDonationDate { get; set; }

    public DateOnly? NextEligibleDate { get; set; }

    public Guid? LocationId { get; set; }

    public Guid? ClosestFacilityId { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string PhoneNumber { get; set; }

    public string Address { get; set; }

    public string CurrentMedications { get; set; }

    public virtual ICollection<BloodDonation> BloodDonations { get; set; } = new List<BloodDonation>();

    public virtual BloodType BloodType { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual MedicalFacility ClosestFacility { get; set; }

    public virtual ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();

    public virtual ICollection<HealthCheck> HealthChecks { get; set; } = new List<HealthCheck>();

    public virtual Location Location { get; set; }

    public virtual ICollection<MedicalFacility> MedicalFacilities { get; set; } = new List<MedicalFacility>();

    public virtual User User { get; set; }
}