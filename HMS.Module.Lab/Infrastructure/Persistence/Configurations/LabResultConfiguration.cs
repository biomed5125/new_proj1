using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;

public sealed class LabResultConfiguration : IEntityTypeConfiguration<myLabResult>
{
    public void Configure(EntityTypeBuilder<myLabResult> b)
    {
        b.ToTable("LabResults");
        b.HasKey(x => x.LabResultId);

        b.Property(x => x.LabResultId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        // Required FKs to request/request-item (existing)
        b.Property(x => x.LabRequestId).IsRequired();
        b.Property(x => x.LabRequestItemId).IsRequired();

        // Instrument/audit info
        b.Property(x => x.DeviceId);
        b.Property(x => x.AccessionNumber).HasMaxLength(64);
        b.Property(x => x.InstrumentTestCode).HasMaxLength(64);
        b.Property(x => x.RawFlag).HasMaxLength(32);

        // Clinical text fields (already in your db)
        b.Property(x => x.LabTestCode).HasMaxLength(40).IsRequired();
        b.Property(x => x.LabTestName).HasMaxLength(120);
        b.Property(x => x.Value).HasMaxLength(128);
        b.Property(x => x.Unit).HasMaxLength(32);
        b.Property(x => x.RefLow).HasPrecision(18, 4);
        b.Property(x => x.RefHigh).HasPrecision(18, 4);

        // Enums as strings
        b.Property(x => x.Flag).HasConversion<string>().HasMaxLength(16);
        b.Property(x => x.Status)
         .HasConversion<string>()
         .HasMaxLength(16)
         .HasDefaultValue(LabResultStatus.Entered);

        // Approvals
        b.Property(x => x.ApprovedByDoctorId);
        b.Property(x => x.ApprovedAtUtc);

        // Audit
        b.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(x => x.CreatedBy).HasMaxLength(64);
        b.Property(x => x.UpdatedAt);
        b.Property(x => x.UpdatedBy).HasMaxLength(64);

        // Indexes
        b.HasIndex(x => x.LabRequestId);
        b.HasIndex(x => x.AccessionNumber);
        b.HasIndex(x => new { x.LabRequestId, x.InstrumentTestCode });

        // One active row per request item (keep if this is your rule)
        b.HasIndex(x => x.LabRequestItemId).IsUnique();

        // --- NEW: FK to LabTests (nullable FIRST – we’ll backfill then tighten)
        b.Property(x => x.LabTestId).IsRequired();
        b.HasOne(x => x.LabTest)
            .WithMany()
            .HasForeignKey(x => x.LabTestId)
            .OnDelete(DeleteBehavior.NoAction);

        // Helpful composite indexes
        b.HasIndex(x => new { x.AccessionNumber, x.LabTestId });
        b.HasIndex(x => new { x.LabRequestId, x.LabTestId });

        // Soft delete
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
