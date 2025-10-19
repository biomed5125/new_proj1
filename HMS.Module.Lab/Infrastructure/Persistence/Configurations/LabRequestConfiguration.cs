using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;

public sealed class LabRequestConfiguration : IEntityTypeConfiguration<myLabRequest>
{
    public void Configure(EntityTypeBuilder<myLabRequest> b)
    {
        b.ToTable("LabRequests");
        b.HasKey(x => x.LabRequestId);

        b.Property(x => x.LabRequestId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        // --- Optional FK to LIS patient (nullable)
        b.HasOne(x => x.LabPatient)
         .WithMany(p => p.Requests)
         .HasForeignKey(x => x.LabPatientId)
         .IsRequired(false)                       // <— make optional
         .OnDelete(DeleteBehavior.Restrict);

        // --- Optional FK to LIS doctor (nullable)
        b.HasOne(x => x.LabDoctor)
         .WithMany(d => d.Requests)
         .HasForeignKey(x => x.LabDoctorId)
         .IsRequired(false)                       // <— make optional
         .OnDelete(DeleteBehavior.Restrict);

        // --- Cross-module IDs are also optional (nullable long?)
        b.Property(x => x.PatientId).IsRequired(false);
        b.Property(x => x.DoctorId).IsRequired(false);

        b.Property(x => x.PatientDisplay).HasMaxLength(240);
        b.Property(x => x.DoctorDisplay).HasMaxLength(240);
        b.Property(x => x.PaidAtUtc);
        b.Property(x => x.Source).HasMaxLength(20);

        b.Property(x => x.Priority).HasMaxLength(40).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(1000);

        b.Property(x => x.OrderNo).HasMaxLength(32).IsRequired();
        b.HasIndex(x => x.OrderNo).IsUnique();

        b.Property(x => x.Status).HasConversion<int>();

        // helpful indexes for history/profile lookups
        b.HasIndex(x => x.LabPatientId);
        b.HasIndex(x => x.LabDoctorId);
        b.HasIndex(x => x.PatientId);
        b.HasIndex(x => x.DoctorId);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
