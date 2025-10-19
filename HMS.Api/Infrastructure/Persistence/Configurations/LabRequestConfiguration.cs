using HMS.Api.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Api.Infrastructure.Persistence.Configurations;

public class LabRequestConfiguration :
    IEntityTypeConfiguration<myLabRequest>, IEntityTypeConfiguration<myLabRequestItem>
{
    public void Configure(EntityTypeBuilder<myLabRequest> b)
    {
        b.ToTable("LabRequests");
        b.HasKey(x => x.LabRequestId);
        b.Property(x => x.LabRequestId).UseHiLo("HmsIdSeq");
        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.Priority).HasMaxLength(20).IsRequired();
        b.Property(x => x.Notes).HasMaxLength(1000);

        // LabRequestConfiguration
        b.Property(x => x.OrderNo).HasMaxLength(32);
        b.HasIndex(x => x.OrderNo).IsUnique()
         .HasFilter("[OrderNo] IS NOT NULL");
        b.HasIndex(x => new { x.PatientId, x.Status });
    }

    public void Configure(EntityTypeBuilder<myLabRequestItem> b)
    {
        b.ToTable("LabRequestItems");
        b.HasKey(x => x.LabRequestItemId);
        b.Property(x => x.LabRequestItemId).UseHiLo("HmsIdSeq");
        b.Property(x => x.Comment).HasMaxLength(500);
        b.HasIndex(x => new { x.LabRequestId, x.LabTestId }).IsUnique();
    }
}
