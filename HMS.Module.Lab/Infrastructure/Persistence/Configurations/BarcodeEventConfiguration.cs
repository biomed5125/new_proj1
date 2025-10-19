// BarcodeEventConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class BarcodeEventConfiguration : IEntityTypeConfiguration<myBarcodeEvent>
    {
        public void Configure(EntityTypeBuilder<myBarcodeEvent> b)
        {
            b.ToTable("BarcodeEvents");
            b.HasKey(x => x.BarcodeEventId);

            // IDs from module sequence
            b.Property(x => x.BarcodeEventId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            b.Property(x => x.AccessionNumber).HasMaxLength(32).IsRequired();
            b.Property(x => x.Event).HasMaxLength(16).IsRequired(); // ISSUED / SCANNED etc.

            // Helpful index for timeline per accession
            b.HasIndex(x => new { x.AccessionNumber, x.At });
        }
    }
}
