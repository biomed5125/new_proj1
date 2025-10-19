// HMS.Core/Persistence/CoreDbContext.cs
using HMS.Core.ReadModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HMS.Core.Infrastructure.Persistence;

public class CoreDbContext : DbContext
{
    public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }

    public DbSet<CorePatient> Patients => Set<CorePatient>();
    public DbSet<CoreAppointment> Appointments => Set<CoreAppointment>();
    public DbSet<CoreWard> Wards => Set<CoreWard>();
    public DbSet<CoreRoomType> RoomTypes => Set<CoreRoomType>();
    public DbSet<CoreWardRoom> WardRooms => Set<CoreWardRoom>();
    public DbSet<CoreAdmission> Admissions => Set<CoreAdmission>();
    // inside CoreDbContext
    public DbSet<CoreDoctor> Doctors => Set<CoreDoctor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // This is what makes your Core*Configuration classes get applied.

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoreDbContext).Assembly);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<CorePatient>(b =>
        {
            b.ToTable("Patients");
            b.HasKey(x => x.PatientId);
            b.Property(x => x.PatientId).ValueGeneratedNever();  // we copy IDs from modules
        });

        modelBuilder.Entity<CoreAppointment>(b =>
        {
            b.ToTable("Appointments");
            b.HasKey(x => x.AppointmentId);
            b.Property(x => x.AppointmentId).ValueGeneratedNever(); // same ID as module
        });

        modelBuilder.Entity<CoreAdmission>(b =>
        {
            b.ToTable("CoreAdmission");
            b.HasKey(x => x.AdmissionId);
            b.Property(x => x.EncounterNo).HasMaxLength(32);
            b.Property(x => x.DiagnosisOnAdmission).HasMaxLength(400);
            b.Property(x => x.Notes).HasMaxLength(1000);
            b.HasIndex(x => x.PatientId);
            b.HasIndex(x => x.AdmittedAtUtc);
        });
        modelBuilder.Entity<CoreWardRoom>(b =>
        {
            b.ToTable("CoreWardRoom");
            b.HasKey(x => x.WardRoomId);
            b.Property(x => x.RoomNumber).HasMaxLength(50);
            b.HasIndex(x => new { x.WardId, x.RoomNumber }).IsUnique();
        });

        modelBuilder.Entity<CoreWard>(b =>
        {
            b.ToTable("CoreWard");
            b.HasKey(x => x.WardId);
            b.Property(x => x.Name).HasMaxLength(200);
        });
        modelBuilder.Entity<CoreRoomType>(b =>
        {
            b.ToTable("CoreRoomType");
            b.HasKey(x => x.RoomTypeId);
            b.Property(x => x.Name).HasMaxLength(200);
        });      
        

        modelBuilder.Entity<CoreDoctor>(b =>
        {
            b.ToTable("CoreDoctor");
            b.HasKey(x => x.DoctorId);
            b.Property(x => x.FirstName).HasMaxLength(100);
            b.Property(x => x.LastName).HasMaxLength(100);
            b.Property(x => x.FullName).HasMaxLength(210);
            b.Property(x => x.LicenseNumber).HasMaxLength(50);
            b.Property(x => x.Specialty).HasMaxLength(120);
            b.Property(x => x.Phone).HasMaxLength(40);
            b.Property(x => x.Email).HasMaxLength(200);
            b.HasIndex(x => x.LicenseNumber).IsUnique();
        });

    }
}
