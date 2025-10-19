using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;

public sealed class LabRequestItemConfiguration : IEntityTypeConfiguration<myLabRequestItem>
{
    public void Configure(EntityTypeBuilder<myLabRequestItem> b)
    {
        b.ToTable("LabRequestItems");

        b.HasKey(x => x.LabRequestItemId);

        b.Property(x => x.LabRequestItemId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        b.Property(x => x.LabTestCode)
         .HasMaxLength(64)
         .IsRequired();

        // Optional copies for history
        b.Property(x => x.LabTestName).HasMaxLength(120);
        b.Property(x => x.LabTestUnit).HasMaxLength(32);

        // Price precision
        b.Property(x => x.LabTestPrice).HasPrecision(10, 2);

        // Audit (defaults)
        b.Property(x => x.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(x => x.CreatedBy).HasMaxLength(64);
        b.Property(x => x.UpdatedBy).HasMaxLength(64);

        // ---------------- Relationships ----------------
        // Keep the back-collection so Include(x => x.Items) works
        b.HasOne(x => x.LabRequest)
         .WithMany(r => r.Items)
         .HasForeignKey(x => x.LabRequestId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.LabTest)
         .WithMany()
         .HasForeignKey(x => x.LabTestId)
         .OnDelete(DeleteBehavior.Restrict);

        // ---------------- Indexes ----------------
        // ✅ Filtered unique index so re-adding a previously soft-deleted test doesn't violate the key
        b.HasIndex(x => new { x.LabRequestId, x.LabTestId })
         .IsUnique()
         .HasDatabaseName("UX_LabRequestItems_Request_Test")
         .HasFilter("[IsDeleted] = 0");

        // (Optional) helper non-unique index for fast lookups by code
        b.HasIndex(x => new { x.LabRequestId, x.LabTestCode })
         .HasDatabaseName("IX_LabRequestItems_Request_TestCode");

        // Global soft-delete filter
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
