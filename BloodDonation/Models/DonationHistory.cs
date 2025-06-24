
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class DonationHistory
{
    public Guid HistoryId { get; set; }

    public Guid? DonorId { get; set; }

    public DateOnly? DonationDate { get; set; }

    public int? Quantity { get; set; }

    public string HealthStatus { get; set; }

    public DateOnly? NextEligibleDate { get; set; }

    public Guid? CertificateId { get; set; }

    public virtual Certificate Certificate { get; set; }

    public virtual Donor Donor { get; set; }
}