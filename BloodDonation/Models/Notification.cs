﻿
#nullable disable
using System;
using System.Collections.Generic;

namespace Models;

public partial class Notification
{
    public Guid NotificationId { get; set; }

    public Guid? UserId { get; set; }

    public string Message { get; set; }

    public string NotificationType { get; set; }

    public DateOnly? SendDate { get; set; }

    public bool? IsRead { get; set; }

    public DateOnly? ScheduledDate { get; set; }

    public Guid? CertificateId { get; set; }

    public virtual Certificate Certificate { get; set; }

    public virtual User User { get; set; }
}