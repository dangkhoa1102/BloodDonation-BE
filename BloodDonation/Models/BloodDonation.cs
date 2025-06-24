#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class BloodDonation
{
    public Guid DonationId { get; set; }

    public Guid? DonorId { get; set; }

    public Guid? RequestId { get; set; }

    public DateOnly? DonationDate { get; set; }

    public int? Quantity { get; set; }

    public string Status { get; set; }

    public string Notes { get; set; }

    public Guid? CertificateId { get; set; }

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual Certificate Certificate { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual Donor Donor { get; set; }

    public virtual BloodRequest Request { get; set; }
}