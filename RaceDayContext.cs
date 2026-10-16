
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data
{
    public class RaceDayContext : DbContext
    {
        public RaceDayContext(
            DbContextOptions<RaceDayContext> options)
            : base(options)
        {
        }

        // Database tables
        public DbSet<User> Users { get; set; }
        public DbSet<Organiser> Organisers { get; set; }
        public DbSet<Participant> Participants { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<MySqlX.XDevAPI.Common.Result> Results { get; set; }

        protected override void OnModelCreating(
            ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // USERS
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserID);

                entity.HasIndex(u => u.Email)
                    .IsUnique();

                entity.Property(u => u.FirstName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(u => u.LastName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(u => u.PasswordHash)
                    .HasMaxLength(255)
                    .IsRequired();

                entity.Property(u => u.Role)
                    .HasMaxLength(20)
                    .IsRequired();

                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Users_Role",
                    "[Role] IN ('Admin', 'Organiser', 'Participant')"));
            });

            // ORGANISERS
            modelBuilder.Entity<Organiser>(entity =>
            {
                entity.HasKey(o => o.OrganiserID);

                entity.HasIndex(o => o.UserID)
                    .IsUnique();

                entity.Property(o => o.OrganisationName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<Organiser>(o => o.UserID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // PARTICIPANTS
            modelBuilder.Entity<Participant>(entity =>
            {
                entity.HasKey(p => p.ParticipantID);

                entity.HasIndex(p => p.UserID)
                    .IsUnique();

                entity.Property(p => p.Phone)
                    .HasMaxLength(20);

                entity.HasOne<User>()
                    .WithOne()
                    .HasForeignKey<Participant>(p => p.UserID)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // EVENTS
            modelBuilder.Entity<Event>(entity =>
            {
                entity.HasKey(e => e.EventID);

                entity.Property(e => e.EventName)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Location)
                    .HasMaxLength(150)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(500);

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Upcoming")
                    .IsRequired();

                entity.HasOne<Organiser>()
                    .WithMany()
                    .HasForeignKey(e => e.OrganiserID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Events_Status",
                    "[Status] IN ('Upcoming', 'Open', 'Completed', 'Cancelled')"));
            });

            // CATEGORIES
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(c => c.CategoryID);

                entity.Property(c => c.CategoryName)
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(c => c.Description)
                    .HasMaxLength(255);

                entity.HasOne<Event>()
                    .WithMany()
                    .HasForeignKey(c => c.EventID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Categories_MaxParticipants",
                    "[MaxParticipants] > 0"));
            });

            // ENROLMENTS
            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.HasKey(e => e.EnrolmentID);

                entity.HasIndex(e => new
                {
                    e.ParticipantID,
                    e.CategoryID
                }).IsUnique();

                entity.Property(e => e.EnrolmentDate)
                    .HasColumnType("date")
                    .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

                entity.Property(e => e.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Active")
                    .IsRequired();

                entity.HasOne<Participant>()
                    .WithMany()
                    .HasForeignKey(e => e.ParticipantID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne<Category>()
                    .WithMany()
                    .HasForeignKey(e => e.CategoryID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t => t.HasCheckConstraint(
                    "CK_Enrolments_Status",
                    "[Status] IN ('Active', 'Cancelled', 'Completed')"));
            });


            // RESULTS
            modelBuilder.Entity<Models.Result>(entity =>
            {
                entity.HasKey(r => r.ResultID);

                entity.HasIndex(r => r.EnrolmentID)
                    .IsUnique();

                entity.Property(r => r.FinishTime)
                    .HasColumnType("time");

                entity.Property(r => r.Status)
                    .HasMaxLength(20)
                    .HasDefaultValue("Finished")
                    .IsRequired();

                entity.HasOne<Enrolment>()
                    .WithOne()
                    .HasForeignKey<Models.Result>(r => r.EnrolmentID)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Results_Position",
                        "[Position] IS NULL OR [Position] > 0");

                    t.HasCheckConstraint(
                        "CK_Results_Status",
                        "[Status] IN ('Finished', 'DNF', 'DNS', 'Disqualified')");
                });
            });

        }
    }
}
