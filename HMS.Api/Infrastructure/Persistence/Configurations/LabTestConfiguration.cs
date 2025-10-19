using HMS.Api.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Api.Infrastructure.Persistence.Configurations;

public class LabTestConfiguration : IEntityTypeConfiguration<myLabTest>
{
    public void Configure(EntityTypeBuilder<myLabTest> b)
    {
        b.ToTable("LabTests");
        b.HasKey(x => x.LabTestId);
        b.Property(x => x.LabTestId).UseHiLo("HmsIdSeq");

        b.Property(x => x.Code).HasMaxLength(40).IsRequired();
        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.SpecimenType).HasMaxLength(50);
        b.Property(x => x.ServiceCode).HasMaxLength(40).IsRequired();
        b.Property(x => x.TatMinutes).IsRequired();

        b.HasIndex(x => x.Code).IsUnique();
    }
}
