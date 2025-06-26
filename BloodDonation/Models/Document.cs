
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class Document
{
    public Guid DocumentId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public string Category { get; set; }

    public DateOnly? UploadDate { get; set; }

    public string FileType { get; set; }

    public int? DownloadCount { get; set; }
}