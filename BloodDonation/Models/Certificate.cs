#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class Certificate
{
    public Guid CertificateId { get; set; }

    public Guid DonorId { get; set; }

    public Guid DonationId { get; set; }

    public Guid StaffId { get; set; }

    public string CertificateNumber { get; set; }

    public DateOnly IssueDate { get; set; }

    public string CertificateType { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? LastModified { get; set; }

    public virtual ICollection<BloodDonation> BloodDonations { get; set; } = new List<BloodDonation>();

    public virtual BloodDonation Donation { get; set; }

    public virtual ICollection<DonationHistory> DonationHistories { get; set; } = new List<DonationHistory>();

    public virtual Donor Donor { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual User Staff { get; set; }
}