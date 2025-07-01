#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class BloodRecipient
{
    public Guid RecipientId { get; set; }

    public Guid? UserId { get; set; }

    public string UrgencyLevel { get; set; }

    public string MedicalCondition { get; set; }

    public string ContactInfo { get; set; }

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public virtual User User { get; set; }
}