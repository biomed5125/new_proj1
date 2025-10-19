// Infrastructure/Persistence/Configurations/myLabPatientProfileConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;

public sealed class LabPatientProfileConfiguration : IEntityTypeConfiguration <myLabPatientProfile>
{
    public void Configure(EntityTypeBuilder<myLabPatientProfile> b)
    {
        b.ToTable("LabPatientProfiles", "dbo");
        b.HasKey(x => x.LabPatientId);

        // IDs from module sequence
        b.Property(x => x.LabPatientId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        b.Property(x => x.AllergyNotes).HasMaxLength(400);
        b.Property(x => x.UpdatedAt).HasPrecision(0);

        // 1:1 with LabPatient (shared PK)
        b.HasOne(x => x.LabPatient)
         .WithOne(p => p.Profile)
         .HasForeignKey<myLabPatientProfile>(x => x.LabPatientId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
