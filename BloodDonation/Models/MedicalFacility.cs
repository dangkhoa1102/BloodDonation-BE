﻿
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class MedicalFacility
{
    public Guid FacilityId { get; set; }

    public string FacilityName { get; set; }

    public string Address { get; set; }

    public string Phone { get; set; }

    public string Email { get; set; }

    public int? Capacity { get; set; }

    public string Specialization { get; set; }

    public string Coordinates { get; set; }

    public Guid? ClosestDonorId { get; set; }

    public virtual Donor ClosestDonor { get; set; }

    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();
}