#nullable disable
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Models;

public partial class BloodDonationSupportContext : DbContext
{
    public BloodDonationSupportContext()
    {
    }

    public BloodDonationSupportContext(DbContextOptions<BloodDonationSupportContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Blog> Blogs { get; set; }

    public virtual DbSet<BloodComponent> BloodComponents { get; set; }

    public virtual DbSet<BloodDonation> BloodDonations { get; set; }

    public virtual DbSet<BloodRecipient> BloodRecipients { get; set; }

    public virtual DbSet<BloodRequest> BloodRequests { get; set; }

    public virtual DbSet<BloodType> BloodTypes { get; set; }

    public virtual DbSet<BloodUnit> BloodUnits { get; set; }

    public virtual DbSet<Certificate> Certificates { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DonationHistory> DonationHistories { get; set; }

    public virtual DbSet<Donor> Donors { get; set; }

    public virtual DbSet<HealthCheck> HealthChecks { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<MedicalFacility> MedicalFacilities { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Report> Reports { get; set; }

    public virtual DbSet<User> Users { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(e => e.BlogId).HasName("PK__Blog__FA0AA70DF494B3C9");

            entity.ToTable("Blog");

            entity.HasIndex(e => e.AuthorId, "IX_Blog_Author");

            entity.Property(e => e.BlogId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("blogID");
            entity.Property(e => e.AuthorId).HasColumnName("authorID");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.PublishDate).HasColumnName("publishDate");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");
            entity.Property(e => e.ViewCount)
                .HasDefaultValue(0)
                .HasColumnName("viewCount");

            entity.HasOne(d => d.Author).WithMany(p => p.Blogs)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("FK_Blog_Author");
        });

        modelBuilder.Entity<BloodComponent>(entity =>
        {
            entity.HasKey(e => e.ComponentId).HasName("PK__BloodCom__667AC126F26CABC1");

            entity.ToTable("BloodComponent");

            entity.Property(e => e.ComponentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("componentID");
            entity.Property(e => e.CompatibilityRules).HasColumnName("compatibilityRules");
            entity.Property(e => e.ComponentName)
                .HasMaxLength(100)
                .HasColumnName("componentName");
            entity.Property(e => e.StorageRequirements)
                .HasMaxLength(500)
                .HasColumnName("storageRequirements");
        });

        modelBuilder.Entity<BloodDonation>(entity =>
        {
            entity.HasKey(e => e.DonationId).HasName("PK__BloodDon__F7F4F433B63F6689");

            entity.ToTable("BloodDonation");

            entity.HasIndex(e => e.CertificateId, "IX_BloodDonation_Certificate");

            entity.Property(e => e.DonationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("donationID");
            entity.Property(e => e.CertificateId).HasColumnName("certificateID");
            entity.Property(e => e.DonationDate).HasColumnName("donationDate");
            entity.Property(e => e.DonorId).HasColumnName("donorID");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.RequestId).HasColumnName("requestID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.Certificate).WithMany(p => p.BloodDonations)
                .HasForeignKey(d => d.CertificateId)
                .HasConstraintName("FK_BloodDonation_Certificate");

            entity.HasOne(d => d.Donor).WithMany(p => p.BloodDonations)
                .HasForeignKey(d => d.DonorId)
                .HasConstraintName("FK_BloodDonation_Donor");

            entity.HasOne(d => d.Request).WithMany(p => p.BloodDonations)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_BloodDonation_Request");
        });

        modelBuilder.Entity<BloodRecipient>(entity =>
        {
            entity.HasKey(e => e.RecipientId).HasName("PK__BloodRec__A9B8B54217264492");

            entity.ToTable("BloodRecipient");

            entity.Property(e => e.RecipientId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("recipientID");
            entity.Property(e => e.ContactInfo)
                .HasMaxLength(500)
                .HasColumnName("contactInfo");
            entity.Property(e => e.MedicalCondition).HasColumnName("medicalCondition");
            entity.Property(e => e.UrgencyLevel)
                .HasMaxLength(50)
                .HasColumnName("urgencyLevel");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.BloodRecipients)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_BloodRecipient_User");
        });

        modelBuilder.Entity<BloodRequest>(entity =>
        {
            entity.HasKey(e => e.RequestId).HasName("PK__BloodReq__E3C5DE512BE91403");

            entity.ToTable("BloodRequest");

            entity.HasIndex(e => e.BloodTypeRequired, "IX_BloodRequest_BloodType");

            entity.HasIndex(e => e.Status, "IX_BloodRequest_Status");

            entity.Property(e => e.RequestId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("requestID");
            entity.Property(e => e.BloodTypeRequired).HasColumnName("bloodTypeRequired");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.QuantityNeeded).HasColumnName("quantityNeeded");
            entity.Property(e => e.RecipientId).HasColumnName("recipientID");
            entity.Property(e => e.RequestDate).HasColumnName("requestDate");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UrgencyLevel)
                .HasMaxLength(50)
                .HasColumnName("urgencyLevel");

            entity.HasOne(d => d.BloodTypeRequiredNavigation).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.BloodTypeRequired)
                .HasConstraintName("FK_BloodRequest_BloodType");

            entity.HasOne(d => d.Recipient).WithMany(p => p.BloodRequests)
                .HasForeignKey(d => d.RecipientId)
                .HasConstraintName("FK_BloodRequest_Recipient");
        });

        modelBuilder.Entity<BloodType>(entity =>
        {
            entity.HasKey(e => e.BloodTypeId).HasName("PK__BloodTyp__C879D794BE3A5040");

            entity.ToTable("BloodType");

            entity.Property(e => e.BloodTypeId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("bloodTypeID");
            entity.Property(e => e.AboType)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("aboType");
            entity.Property(e => e.Description)
                .HasMaxLength(500)
                .HasColumnName("description");
            entity.Property(e => e.RhFactor)
                .IsRequired()
                .HasMaxLength(10)
                .HasColumnName("rhFactor");
        });

        modelBuilder.Entity<BloodUnit>(entity =>
        {
            entity.HasKey(e => e.UnitId).HasName("PK__BloodUni__55D79215DA87E384");

            entity.ToTable("BloodUnit");

            entity.HasIndex(e => e.BloodTypeId, "IX_BloodUnit_BloodType");

            entity.HasIndex(e => e.ExpiryDate, "IX_BloodUnit_ExpiryDate");

            entity.HasIndex(e => e.RequestId, "IX_BloodUnit_Request");

            entity.HasIndex(e => e.Status, "IX_BloodUnit_Status");

            entity.Property(e => e.UnitId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("unitID");
            entity.Property(e => e.BloodTypeId).HasColumnName("bloodTypeID");
            entity.Property(e => e.ComponentType).HasColumnName("componentType");
            entity.Property(e => e.DonationId).HasColumnName("donationID");
            entity.Property(e => e.ExpiryDate).HasColumnName("expiryDate");
            entity.Property(e => e.Quantity)
                .HasDefaultValue(450)
                .HasColumnName("quantity");
            entity.Property(e => e.RequestId).HasColumnName("requestID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");

            entity.HasOne(d => d.BloodType).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.BloodTypeId)
                .HasConstraintName("FK_BloodUnit_BloodType");

            entity.HasOne(d => d.ComponentTypeNavigation).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.ComponentType)
                .HasConstraintName("FK_BloodUnit_Component");

            entity.HasOne(d => d.Donation).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.DonationId)
                .HasConstraintName("FK_BloodUnit_Donation");

            entity.HasOne(d => d.Request).WithMany(p => p.BloodUnits)
                .HasForeignKey(d => d.RequestId)
                .HasConstraintName("FK_BloodUnit_Request");
        });

        modelBuilder.Entity<Certificate>(entity =>
        {
            entity.HasKey(e => e.CertificateId).HasName("PK__Certific__A15CBE8E388020DB");

            entity.ToTable("Certificate");

            entity.HasIndex(e => e.DonationId, "IX_Certificate_Donation");

            entity.HasIndex(e => e.DonorId, "IX_Certificate_Donor");

            entity.HasIndex(e => e.IssueDate, "IX_Certificate_IssueDate");

            entity.HasIndex(e => e.CertificateNumber, "IX_Certificate_Number");

            entity.HasIndex(e => e.StaffId, "IX_Certificate_Staff");

            entity.HasIndex(e => e.CertificateNumber, "UQ__Certific__410CE51220B3E296").IsUnique();

            entity.Property(e => e.CertificateId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("certificateID");
            entity.Property(e => e.CertificateNumber)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("certificateNumber");
            entity.Property(e => e.CertificateType)
                .HasMaxLength(50)
                .HasDefaultValue("Blood Donation")
                .HasColumnName("certificateType");
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdDate");
            entity.Property(e => e.DonationId).HasColumnName("donationID");
            entity.Property(e => e.DonorId).HasColumnName("donorID");
            entity.Property(e => e.IssueDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("issueDate");
            entity.Property(e => e.LastModified)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("lastModified");
            entity.Property(e => e.StaffId).HasColumnName("staffID");

            entity.HasOne(d => d.Donation).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.DonationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificate_Donation");

            entity.HasOne(d => d.Donor).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificate_Donor");

            entity.HasOne(d => d.Staff).WithMany(p => p.Certificates)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Certificate_Staff");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("PK__Document__EFAAADE5699BA8E4");

            entity.ToTable("Document");

            entity.Property(e => e.DocumentId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("documentID");
            entity.Property(e => e.Category)
                .HasMaxLength(100)
                .HasColumnName("category");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.DownloadCount)
                .HasDefaultValue(0)
                .HasColumnName("downloadCount");
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .HasColumnName("fileType");
            entity.Property(e => e.Title)
                .HasMaxLength(500)
                .HasColumnName("title");
            entity.Property(e => e.UploadDate).HasColumnName("uploadDate");
        });

        modelBuilder.Entity<DonationHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId).HasName("PK__Donation__19BDBDB36E09FAAA");

            entity.ToTable("DonationHistory");

            entity.HasIndex(e => e.CertificateId, "IX_DonationHistory_Certificate");

            entity.Property(e => e.HistoryId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("historyID");
            entity.Property(e => e.CertificateId).HasColumnName("certificateID");
            entity.Property(e => e.DonationDate).HasColumnName("donationDate");
            entity.Property(e => e.DonorId).HasColumnName("donorID");
            entity.Property(e => e.HealthStatus).HasColumnName("healthStatus");
            entity.Property(e => e.NextEligibleDate).HasColumnName("nextEligibleDate");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Certificate).WithMany(p => p.DonationHistories)
                .HasForeignKey(d => d.CertificateId)
                .HasConstraintName("FK_DonationHistory_Certificate");

            entity.HasOne(d => d.Donor).WithMany(p => p.DonationHistories)
                .HasForeignKey(d => d.DonorId)
                .HasConstraintName("FK_DonationHistory_Donor");
        });

        modelBuilder.Entity<Donor>(entity =>
        {
            entity.HasKey(e => e.DonorId).HasName("PK__Donor__A595D73182E52831");

            entity.ToTable("Donor");

            entity.HasIndex(e => e.BloodTypeId, "IX_Donor_BloodType");

            entity.HasIndex(e => e.LocationId, "IX_Donor_Location");

            entity.HasIndex(e => e.UserId, "IX_Donor_User");

            entity.Property(e => e.DonorId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("donorID");
            entity.Property(e => e.BloodTypeId).HasColumnName("bloodTypeID");
            entity.Property(e => e.ClosestFacilityId).HasColumnName("closestFacilityID");
            entity.Property(e => e.IsAvailable).HasColumnName("isAvailable");
            entity.Property(e => e.LastDonationDate).HasColumnName("lastDonationDate");
            entity.Property(e => e.LocationId).HasColumnName("locationID");
            entity.Property(e => e.NextEligibleDate).HasColumnName("nextEligibleDate");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.BloodType).WithMany(p => p.Donors)
                .HasForeignKey(d => d.BloodTypeId)
                .HasConstraintName("FK_Donor_BloodType");

            entity.HasOne(d => d.ClosestFacility).WithMany(p => p.Donors)
                .HasForeignKey(d => d.ClosestFacilityId)
                .HasConstraintName("FK_Donor_MedicalFacility");

            entity.HasOne(d => d.Location).WithMany(p => p.Donors)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_Donor_Location");

            entity.HasOne(d => d.User).WithMany(p => p.Donors)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Donor_User");
        });

        modelBuilder.Entity<HealthCheck>(entity =>
        {
            entity.HasKey(e => e.HealthCheckId).HasName("PK__HealthCh__31AFA16E0C212F82");

            entity.ToTable("HealthCheck");

            entity.HasIndex(e => e.HealthCheckDate, "IX_HealthCheck_Date");

            entity.HasIndex(e => e.DonorId, "IX_HealthCheck_Donor");

            entity.HasIndex(e => e.HealthCheckStatus, "IX_HealthCheck_Status");

            entity.Property(e => e.HealthCheckId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("healthCheckID");
            entity.Property(e => e.Allergies).HasColumnName("allergies");
            entity.Property(e => e.BloodPressure)
                .HasMaxLength(20)
                .HasColumnName("blood_pressure");
            entity.Property(e => e.CurrentMedications).HasColumnName("currentMedications");
            entity.Property(e => e.DonorId).HasColumnName("donorID");
            entity.Property(e => e.HealthCheckDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("HealthCheck_Date");
            entity.Property(e => e.HealthCheckStatus)
                .HasMaxLength(50)
                .HasColumnName("HealthCheck_Status");
            entity.Property(e => e.HeartRate).HasColumnName("heartRate");
            entity.Property(e => e.Height).HasColumnName("height");
            entity.Property(e => e.MedicalHistory).HasColumnName("medicalHistory");
            entity.Property(e => e.Temperature).HasColumnName("temperature");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.Donor).WithMany(p => p.HealthChecks)
                .HasForeignKey(d => d.DonorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HealthCheck_Donor");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__30646B0E7DF4F6FC");

            entity.ToTable("Location");

            entity.Property(e => e.LocationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("locationID");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.Latitude).HasColumnName("latitude");
            entity.Property(e => e.Longitude).HasColumnName("longitude");
        });

        modelBuilder.Entity<MedicalFacility>(entity =>
        {
            entity.HasKey(e => e.FacilityId).HasName("PK__MedicalF__AA5481849741A8E3");

            entity.ToTable("MedicalFacility");

            entity.Property(e => e.FacilityId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("facilityID");
            entity.Property(e => e.Address)
                .HasMaxLength(500)
                .HasColumnName("address");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.ClosestDonorId).HasColumnName("closestDonorID");
            entity.Property(e => e.Coordinates)
                .HasMaxLength(100)
                .HasColumnName("coordinates");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FacilityName)
                .HasMaxLength(255)
                .HasColumnName("facilityName");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Specialization)
                .HasMaxLength(255)
                .HasColumnName("specialization");

            entity.HasOne(d => d.ClosestDonor).WithMany(p => p.MedicalFacilities)
                .HasForeignKey(d => d.ClosestDonorId)
                .HasConstraintName("FK_MedicalFacility_Donor");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__4BA5CE8910416400");

            entity.ToTable("Notification");

            entity.HasIndex(e => e.CertificateId, "IX_Notification_Certificate");

            entity.HasIndex(e => e.IsRead, "IX_Notification_IsRead");

            entity.HasIndex(e => e.UserId, "IX_Notification_User");

            entity.Property(e => e.NotificationId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("notificationID");
            entity.Property(e => e.CertificateId).HasColumnName("certificateID");
            entity.Property(e => e.IsRead)
                .HasDefaultValue(false)
                .HasColumnName("isRead");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.NotificationType)
                .HasMaxLength(50)
                .HasColumnName("notificationType");
            entity.Property(e => e.ScheduledDate).HasColumnName("scheduledDate");
            entity.Property(e => e.SendDate).HasColumnName("sendDate");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Certificate).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.CertificateId)
                .HasConstraintName("FK_Notification_Certificate");

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Notification_User");
        });

        modelBuilder.Entity<Report>(entity =>
        {
            entity.HasKey(e => e.ReportId).HasName("PK__Report__1C9B4ECDF896B590");

            entity.ToTable("Report");

            entity.HasIndex(e => e.GeneratedBy, "IX_Report_GeneratedBy");

            entity.Property(e => e.ReportId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("reportID");
            entity.Property(e => e.Data).HasColumnName("data");
            entity.Property(e => e.GeneratedBy).HasColumnName("generatedBy");
            entity.Property(e => e.GenerationDate).HasColumnName("generationDate");
            entity.Property(e => e.Parameters).HasColumnName("parameters");
            entity.Property(e => e.ReportType)
                .HasMaxLength(100)
                .HasColumnName("reportType");

            entity.HasOne(d => d.GeneratedByNavigation).WithMany(p => p.Reports)
                .HasForeignKey(d => d.GeneratedBy)
                .HasConstraintName("FK_Report_User");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__users__CB9A1CDFB74F38F1");

            entity.ToTable("users");

            entity.Property(e => e.UserId)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("userID");
            entity.Property(e => e.DateOfBirth).HasColumnName("dateOfBirth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("fullName");
            entity.Property(e => e.Password)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.UserIdCard)
                .HasMaxLength(20)
                .HasColumnName("userIdCard");
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}