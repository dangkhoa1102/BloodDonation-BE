#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class BloodComponent
{
    public Guid ComponentId { get; set; }

    public string ComponentName { get; set; }

    public string CompatibilityRules { get; set; }

    public string StorageRequirements { get; set; }

    public virtual ICollection<BloodUnit> BloodUnits { get; set; } = new List<BloodUnit>();
}