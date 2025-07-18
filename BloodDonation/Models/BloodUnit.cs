﻿#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class BloodUnit
{
    public Guid UnitId { get; set; }

    public Guid? DonationId { get; set; }

    public Guid? BloodTypeId { get; set; }

    public Guid? ComponentType { get; set; }

    public DateOnly? ExpiryDate { get; set; }

    public string Status { get; set; }

    public int Quantity { get; set; }

    public Guid? RequestId { get; set; }

    public virtual BloodType BloodType { get; set; }

    public virtual BloodComponent ComponentTypeNavigation { get; set; }

    public virtual BloodDonation Donation { get; set; }

    public virtual BloodRequest Request { get; set; }
}