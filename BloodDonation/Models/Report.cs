
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class Report
{
    public Guid ReportId { get; set; }

    public string ReportType { get; set; }

    public Guid? GeneratedBy { get; set; }

    public DateOnly? GenerationDate { get; set; }

    public string Data { get; set; }

    public string Parameters { get; set; }

    public virtual User GeneratedByNavigation { get; set; }
}