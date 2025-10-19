using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;

public sealed class LabTestInstrumentMapConfiguration : IEntityTypeConfiguration<myInstrumentTestMap>
{
    public void Configure(EntityTypeBuilder<myInstrumentTestMap> b)
    {
        b.ToTable("InstrumentTestMaps");
        b.HasKey(x => x.InstrumentTestMapId);
        b.Property(x => x.InstrumentTestCode).HasMaxLength(64).IsRequired();
        b.HasIndex(x => new { x.DeviceId, x.InstrumentTestCode }).IsUnique();
    }
}
