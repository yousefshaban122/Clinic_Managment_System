using Clinic.Domain.Entities;
using Clinic.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.data
{
    public class App_Context : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Appointment> Appointments => Set<Appointment>();
        public DbSet<Doctors> Doctors => Set<Doctors>();
        public DbSet<Patients> Patients => Set<Patients>();
        public DbSet<Invoice> Invoices => Set<Invoice>();
        public DbSet<Medical_Records> MedicalRecords => Set<Medical_Records>();

        public App_Context(DbContextOptions<App_Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Doctors>().ToTable("Doctors");
            modelBuilder.Entity<Patients>().ToTable("Patients");
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
            modelBuilder.Entity<Invoice>().ToTable("Invoices");
            modelBuilder.Entity<Medical_Records>().ToTable("MedicalRecords");

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Medical_Records>()
                .HasOne(m => m.Appointment)
                .WithOne(a => a.MedicalRecord)
                .HasForeignKey<Medical_Records>(m => m.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Appointment)
                .WithOne(a => a.Invoice)
                .HasForeignKey<Invoice>(i => i.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Invoice>()
                .HasIndex(i => i.AppointmentId)
                .IsUnique();

            modelBuilder.Entity<Medical_Records>()
                .HasIndex(m => m.AppointmentId)
                .IsUnique();

            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Doctors>()
                .HasIndex(d => d.UserId)
                .IsUnique()
                .HasFilter("[UserId] IS NOT NULL");
        }
    }
}
