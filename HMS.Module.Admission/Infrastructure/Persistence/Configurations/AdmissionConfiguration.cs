using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Features.Admission.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Admission.Infrastructure.Persistence.Configurations;

public sealed class AdmissionConfiguration : IEntityTypeConfiguration<myAdmission>
{
    public void Configure(EntityTypeBuilder<myAdmission> b)
    {
        b.ToTable("Admissions");
        b.HasKey(x => x.AdmissionId);
        b.Property(x => x.AdmissionId).UseHiLo("HmsIdSeq");
        //b.HasKey(x => x.AdmissionId);
        //b.Property(x => x.AdmissionId)
        //    .ValueGeneratedOnAdd()
        //    .HasDefaultValueSql("NEXT VALUE FOR [dbo].[HmsIdSeq]");

        b.Property(x => x.EncounterNo).HasMaxLength(32).IsRequired();
        b.HasIndex(x => x.EncounterNo).IsUnique();

        b.Property(x => x.Status).HasConversion<int>().IsRequired();
        b.Property(x => x.DiagnosisOnAdmission).HasMaxLength(256);
        b.Property(x => x.Notes).HasMaxLength(1000);

        b.HasIndex(x => new { x.PatientId, x.Status, x.AdmittedAtUtc });
    }
}
