// ReferenceRangeConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class ReferenceRangeConfiguration : IEntityTypeConfiguration<myReferenceRange>
    {
        public void Configure(EntityTypeBuilder<myReferenceRange> b)
        {
            b.ToTable("ReferenceRanges");
            b.HasKey(x => x.ReferenceRangeId);

            b.Property(x => x.ReferenceRangeId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            b.Property(x => x.RefLow).HasPrecision(10, 2);
            b.Property(x => x.RefHigh).HasPrecision(10, 2);
            b.Property(x => x.Note).HasMaxLength(120);

            b.Property(x => x.LabTestId).IsRequired(false);

            b.HasOne(x => x.LabTest)
             .WithMany() // or .WithMany(t => t.ReferenceRanges)
             .HasForeignKey(x => x.LabTestId)
             .OnDelete(DeleteBehavior.Restrict);

            b.HasIndex(x => x.LabTestId);
        }
    }
}
