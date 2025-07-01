#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class Blog
{
    public Guid BlogId { get; set; }

    public Guid? AuthorId { get; set; }

    public string Title { get; set; }

    public string Content { get; set; }

    public DateOnly? PublishDate { get; set; }

    public string Category { get; set; }

    public virtual User Author { get; set; }
}