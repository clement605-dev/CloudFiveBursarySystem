using CloudFiveBursarySystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CloudFiveBursarySystem.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        // Applicant-related database tables.
        public DbSet<Applicant> Applicants => Set<Applicant>();

        // Bursary programme database table.
        public DbSet<BursaryProgramme> BursaryProgrammes => Set<BursaryProgramme>();

        // Bursary application database table.
        public DbSet<BursaryApplication> BursaryApplications => Set<BursaryApplication>();

        // Beneficiary database table.
        public DbSet<Beneficiary> Beneficiaries => Set<Beneficiary>();

        // Payment database table.
        public DbSet<Payment> Payments => Set<Payment>();

        // Applicant supporting documents database table.
        public DbSet<ApplicantDocument> ApplicantDocuments => Set<ApplicantDocument>();

        // Applicant notifications database table.
        public DbSet<Notification> Notifications => Set<Notification>();

        // Administrator activity log database table.
        public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // APPLICANT RELATIONSHIP
            builder.Entity<Applicant>()
                .HasOne<IdentityUser>()
                .WithOne()
                .HasForeignKey<Applicant>(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Applicant>()
                .HasIndex(a => a.UserId)
                .IsUnique();

            builder.Entity<Applicant>()
                .HasIndex(a => a.IDNumber)
                .IsUnique();


            // BURSARY APPLICATION RELATIONSHIPS
            builder.Entity<BursaryApplication>()
                .HasOne(a => a.Applicant)
                .WithMany()
                .HasForeignKey(a => a.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<BursaryApplication>()
                .HasOne(a => a.BursaryProgramme)
                .WithMany()
                .HasForeignKey(a => a.BursaryProgrammeId)
                .OnDelete(DeleteBehavior.Restrict);


            // BENEFICIARY RELATIONSHIP
            builder.Entity<Beneficiary>()
                .HasOne(b => b.BursaryApplication)
                .WithOne()
                .HasForeignKey<Beneficiary>(b => b.BursaryApplicationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Beneficiary>()
                .HasIndex(b => b.BursaryApplicationId)
                .IsUnique();

            builder.Entity<Beneficiary>()
                .Property(b => b.FundingAmount)
                .HasPrecision(18, 2);


            // BURSARY PROGRAMME CONFIGURATION
            builder.Entity<BursaryProgramme>()
                .Property(p => p.FundingAmount)
                .HasPrecision(18, 2);


            // PAYMENT RELATIONSHIP
            builder.Entity<Payment>()
                .HasOne(p => p.Beneficiary)
                .WithMany()
                .HasForeignKey(p => p.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasIndex(p => p.PaymentReference)
                .IsUnique();

            builder.Entity<Payment>()
                .Property(p => p.PaymentAmount)
                .HasPrecision(18, 2);


            // APPLICANT DOCUMENT RELATIONSHIP
            builder.Entity<ApplicantDocument>()
                .HasOne(d => d.Applicant)
                .WithMany()
                .HasForeignKey(d => d.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);


            // NOTIFICATION RELATIONSHIP
            builder.Entity<Notification>()
                .HasOne(n => n.Applicant)
                .WithMany()
                .HasForeignKey(n => n.ApplicantId)
                .OnDelete(DeleteBehavior.Restrict);

            // Index to make applicant notification lookups faster.
            builder.Entity<Notification>()
                .HasIndex(n => new { n.ApplicantId, n.IsRead });


            // APPLICANT AND ADMINISTRATOR ACTIVITY LOG CONFIGURATION

            // Index to make user activity searches faster.
            builder.Entity<ActivityLog>()
                .HasIndex(a => a.UserId);

            // Index to filter activities by user role.
            builder.Entity<ActivityLog>()
                .HasIndex(a => a.UserRole);

            // Index to help retrieve activities in date order.
            builder.Entity<ActivityLog>()
                .HasIndex(a => a.DateCreated);
        }
    }
}