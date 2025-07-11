using InteractiveRobot.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InteractiveRobot.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for your models
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<Child> Children { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<DoctorRating> DoctorRatings { get; set; }
        public DbSet<SuggestedGame> SuggestedGames { get; set; }
        public DbSet<ChildGameSuggestion> ChildGameSuggestions { get; set; }
        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<DoctorSpecialty> DoctorSpecialties { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relationship: Parent (User) <---> Children
            modelBuilder.Entity<Child>()
                .HasOne(c => c.Parent)
                .WithMany(p => p.Children)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Doctor (User) <---> Ratings
            modelBuilder.Entity<DoctorRating>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.DoctorRatings)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Parent (User) <---> Ratings
            modelBuilder.Entity<DoctorRating>()
                .HasOne(r => r.Parent)
                .WithMany()
                .HasForeignKey(r => r.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Doctor <-> Specialty (Many to Many)
            modelBuilder.Entity<DoctorSpecialty>()
                .HasKey(ds => new { ds.DoctorId, ds.SpecialtyId });

            modelBuilder.Entity<DoctorSpecialty>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSpecialties)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DoctorSpecialty>()
                .HasOne(ds => ds.Specialty)
                .WithMany(s => s.DoctorSpecialties)
                .HasForeignKey(ds => ds.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}