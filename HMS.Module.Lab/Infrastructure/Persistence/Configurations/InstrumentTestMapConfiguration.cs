// InstrumentTestMapConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class InstrumentTestMapConfiguration : IEntityTypeConfiguration<myInstrumentTestMap>
    {
        public void Configure(EntityTypeBuilder<myInstrumentTestMap> b)
        {
            b.ToTable("InstrumentTestMap");
            b.HasKey(x => x.InstrumentTestMapId);
            b.Property(x => x.InstrumentTestMapId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            b.Property(x => x.InstrumentTestCode).HasMaxLength(80).IsRequired();
            
            b.Property(x => x.LabTestCode).HasMaxLength(64).IsRequired();

            b.HasIndex(x => new { x.DeviceId, x.InstrumentTestCode }).IsUnique();

            b.HasIndex(x => x.LabTestCode);
            // FK to LabTests
            b.HasOne<myLabTest>()
             .WithMany()
             .HasForeignKey(x => x.LabTestId)
             .OnDelete(DeleteBehavior.Restrict);

            b.Property(x => x.CreatedBy).HasMaxLength(60);
            b.Property(x => x.UpdatedBy).HasMaxLength(60);

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }

}
