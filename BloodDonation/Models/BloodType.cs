﻿#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class BloodType
{
    public Guid BloodTypeId { get; set; }

    public string AboType { get; set; }

    public string RhFactor { get; set; }

    public string Description { get; set; }

    public virtual ICollection<BloodRequest> BloodRequests { get; set; } = new List<BloodRequest>();

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();

    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();
}