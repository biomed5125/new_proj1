// InstrumentConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class InstrumentConfiguration : IEntityTypeConfiguration<myLabInstrument>
    {
        public void Configure(EntityTypeBuilder<myLabInstrument> b)
        {
            b.ToTable("LabInstruments");
            b.HasKey(x => x.LabInstrumentId);

            b.Property(x => x.LabInstrumentId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            b.Property(x => x.Name).HasMaxLength(120).IsRequired();
            b.Property(x => x.MakeModel).HasMaxLength(120);
            b.Property(x => x.ConnectionType).HasMaxLength(40);
            b.Property(x => x.Host).HasMaxLength(120);

            b.Property(x => x.IsActive).HasDefaultValue(true);

            b.HasIndex(x => x.Name).IsUnique();

            b.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
