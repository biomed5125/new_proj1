using HMS.Api.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Api.Infrastructure.Persistence.Configurations;

public class LabSampleConfiguration : IEntityTypeConfiguration<myLabSample>
{
    public void Configure(EntityTypeBuilder<myLabSample> b)
    {
        b.ToTable("LabSamples");
        b.HasKey(x => x.LabSampleId);
        b.Property(x => x.LabSampleId).UseHiLo("HmsIdSeq");

        //b.Property(x => x.AccessionNumber).HasMaxLength(50).IsRequired();
        b.Property(x => x.AccessionNumber).HasMaxLength(16).IsRequired();
        b.HasIndex(x => x.AccessionNumber).IsUnique();

        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.CollectedAtUtc).IsRequired();
        b.Property(x => x.RejectReason).HasMaxLength(500);
    }
}
