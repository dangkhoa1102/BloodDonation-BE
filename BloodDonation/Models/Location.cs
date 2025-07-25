﻿
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class Location
{
    public Guid LocationId { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public string Address { get; set; }

    public string City { get; set; }

    public string District { get; set; }

    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();
}