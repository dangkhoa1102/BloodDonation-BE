
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class User
{
    public Guid UserId { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public string UserIdCard { get; set; }

    public string Email { get; set; }

    public string FullName { get; set; }

    public string Phone { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string Role { get; set; }

    public virtual ICollection<Blog> Blogs { get; set; } = new List<Blog>();

    public virtual ICollection<BloodRecipient> BloodRecipients { get; set; } = new List<BloodRecipient>();

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual ICollection<Donor> Donors { get; set; } = new List<Donor>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
}