using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;

public sealed class InstrumentMappingConfiguration : IEntityTypeConfiguration<myInstrumentMapping>
{
    public void Configure(EntityTypeBuilder<myInstrumentMapping> b)
    {
        b.ToTable("InstrumentMappings");
        b.HasKey(x => x.InstrumentMappingId);
        b.Property(x => x.InstrumentMappingId).ValueGeneratedOnAdd();

        b.Property(x => x.InstrumentCode).HasMaxLength(80).IsRequired();
        b.Property(x => x.LocalTestCode).HasMaxLength(40);
        b.Property(x => x.Notes).HasMaxLength(200);

        b.HasIndex(x => new { x.InstrumentMappingId, x.InstrumentCode }).IsUnique();

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
