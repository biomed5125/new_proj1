using HMS.Module.Admission.Features.Admission.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Admission.Infrastructure.Persistence.Configurations;

public sealed class WardConfiguration : IEntityTypeConfiguration<myWard>
{
    public void Configure(EntityTypeBuilder<myWard> b)
    {
        b.ToTable("Wards");

        b.HasKey(x => x.WardId);
        b.Property(x => x.WardId).UseHiLo("HmsIdSeq");
        //b.HasKey(x => x.WardId);
        //b.Property(x => x.WardId)
        //    .ValueGeneratedOnAdd()
        //    .HasDefaultValueSql("NEXT VALUE FOR [dbo].[HmsIdSeq]");

        b.Property(x => x.Name).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
    }
}
