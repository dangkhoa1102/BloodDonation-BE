﻿#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class BloodRequest
{
    public Guid RequestId { get; set; }

    public Guid? RecipientId { get; set; }

    public Guid? BloodTypeRequired { get; set; }

    public int? QuantityNeeded { get; set; }

    public string UrgencyLevel { get; set; }

    public DateOnly? RequestDate { get; set; }

    public string Status { get; set; }

    public string Description { get; set; }

    public virtual ICollection<BloodDonation> BloodDonations { get; set; } = new List<BloodDonation>();

    public virtual BloodType BloodTypeRequiredNavigation { get; set; }

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual BloodRecipient Recipient { get; set; }
}