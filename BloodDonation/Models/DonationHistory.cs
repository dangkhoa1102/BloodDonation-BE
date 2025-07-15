
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class DonationHistory
{
    public Guid HistoryId { get; set; }

    public Guid? DonorId { get; set; }

    public DateTime? DonationDate { get; set; }

    public int? Quantity { get; set; }

    public string HealthStatus { get; set; }

    public DateTime? NextEligibleDate { get; set; }

    public Guid? CertificateId { get; set; }

    public virtual Certificate Certificate { get; set; }

    public virtual Donor Donor { get; set; }
}