using HMS.Api.Features.Lab.Models.Entities;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Api.Infrastructure.Persistence.Configurations;

public class LabResultConfiguration : IEntityTypeConfiguration<myLabResult>
{
    public void Configure(EntityTypeBuilder<myLabResult> b)
    {
        b.ToTable("LabResults");
        b.HasKey(x => x.LabResultId);
        b.Property(x => x.LabResultId).UseIdentityColumn();

        b.Property(x => x.Value).HasMaxLength(200);
        b.Property(x => x.Unit).HasMaxLength(50);
        b.Property(x => x.Flag).HasMaxLength(20);
        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.RefLow).HasPrecision(18, 2);
        b.Property(x => x.RefHigh).HasPrecision(18, 2);

        b.HasIndex(x => new { x.LabRequestId, x.LabRequestItemId }).IsUnique(); // one result per item
    }
}
