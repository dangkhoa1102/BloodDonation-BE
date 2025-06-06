using Blood_Donation_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Blood_Donation_System.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BloodType> BloodTypes { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<BloodComponent> BloodComponents { get; set; }
        public DbSet<MedicalFacility> MedicalFacilities { get; set; }
        public DbSet<Donor> Donors { get; set; }
        public DbSet<BloodRecipient> BloodRecipients { get; set; }
        public DbSet<BloodRequest> BloodRequests { get; set; }
        public DbSet<BloodDonation> BloodDonations { get; set; }
        public DbSet<BloodUnit> BloodUnits { get; set; }
        public DbSet<DonationHistory> DonationHistories { get; set; }
        public DbSet<Blog> Blog { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships
            modelBuilder.Entity<Donor>()
                .HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<Donor>()
                .HasOne(d => d.BloodType)
                .WithMany()
                .HasForeignKey(d => d.BloodTypeId);

            modelBuilder.Entity<Donor>()
                .HasOne(d => d.Location)
                .WithMany()
                .HasForeignKey(d => d.LocationId);

            modelBuilder.Entity<Donor>()
                .HasOne(d => d.ClosestFacility)
                .WithMany()
                .HasForeignKey(d => d.ClosestFacilityId);

            modelBuilder.Entity<BloodRecipient>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);

            modelBuilder.Entity<BloodRequest>()
                .HasOne(r => r.Recipient)
                .WithMany()
                .HasForeignKey(r => r.RecipientId);

            modelBuilder.Entity<BloodRequest>()
                .HasOne(r => r.BloodTypeRequired)
                .WithMany()
                .HasForeignKey(r => r.BloodTypeRequiredId);

            modelBuilder.Entity<BloodDonation>()
                .HasOne(d => d.Donor)
                .WithMany()
                .HasForeignKey(d => d.DonorId);

            modelBuilder.Entity<BloodDonation>()
                .HasOne(d => d.Request)
                .WithMany()
                .HasForeignKey(d => d.RequestId);

            modelBuilder.Entity<BloodUnit>()
                .HasOne(u => u.Donation)
                .WithMany()
                .HasForeignKey(u => u.DonationId);

            modelBuilder.Entity<BloodUnit>()
                .HasOne(u => u.BloodType)
                .WithMany()
                .HasForeignKey(u => u.BloodTypeId);

            modelBuilder.Entity<BloodUnit>()
                .HasOne(u => u.ComponentType)
                .WithMany()
                .HasForeignKey(u => u.ComponentTypeId);

            modelBuilder.Entity<DonationHistory>()
                .HasOne(h => h.Donor)
                .WithMany()
                .HasForeignKey(h => h.DonorId);

            modelBuilder.Entity<Blog>()
                .HasOne(b => b.Author)
                .WithMany()
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId);

            modelBuilder.Entity<Report>()
                .HasOne(r => r.GeneratedBy)
                .WithMany()
                .HasForeignKey(r => r.GeneratedById);

            modelBuilder.Entity<MedicalFacility>()
                .HasOne(f => f.ClosestDonor)
                .WithMany()
                .HasForeignKey(f => f.ClosestDonorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
} 