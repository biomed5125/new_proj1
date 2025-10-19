using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Features.Lab.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;
public sealed class LabSampleConfiguration : IEntityTypeConfiguration<myLabSample>
{
    public void Configure(EntityTypeBuilder<myLabSample> b)
    {
        b.ToTable("LabSamples");
        b.HasKey(x => x.LabSampleId);

        b.Property(x => x.LabSampleId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        b.Property(x => x.AccessionNumber).HasMaxLength(50).IsRequired();

        // Keep a single unique index
        b.HasIndex(x => x.AccessionNumber).IsUnique();

        b.Property(x => x.Status)
         .HasConversion<int>()
         .HasDefaultValue(LabSampleStatus.Collected);

        b.HasOne(x => x.Request)
         .WithMany()
         .HasForeignKey(x => x.LabRequestId);

        b.HasIndex(x => x.LabRequestId)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0")
            .HasDatabaseName("IX_LabSamples_LabRequestId");


        b.HasQueryFilter(x => !x.IsDeleted);

        // NEW: default false in the DB
        b.Property(x => x.LabelPrinted).HasDefaultValue(false).IsRequired();
    }
}
