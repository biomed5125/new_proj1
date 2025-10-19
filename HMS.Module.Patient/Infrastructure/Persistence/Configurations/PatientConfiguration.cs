using HMS.Module.Patient.Features.Patient.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Patient.Infrastructure.Persistence.Configurations;

public class PatientConfiguration : IEntityTypeConfiguration<myPatient>
{
    public void Configure(EntityTypeBuilder<myPatient> b)
    {
        b.ToTable("Patients");                 // separate DB → no schema needed
        b.HasKey(x => x.PatientId);
        b.Property(x => x.PatientId).UseHiLo("HmsIdSeq");
        //b.HasKey(x => x.PatientId);
        //b.Property(x => x.PatientId)
        //    .ValueGeneratedOnAdd()
        //    .HasDefaultValueSql("NEXT VALUE FOR [dbo].[HmsIdSeq]");

        b.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
        b.Property(x => x.LastName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Phone).HasMaxLength(32);
        b.Property(x => x.Email).HasMaxLength(128);
        b.Property(x => x.Gender).HasMaxLength(16);
        b.Property(x => x.DateOfBirth).HasColumnType("date");

        b.Property(x => x.Mrn).HasMaxLength(20);
        b.HasIndex(x => x.Mrn).IsUnique().HasFilter("[Mrn] IS NOT NULL");

        b.HasIndex(x => new { x.FirstName, x.LastName, x.Phone, x.Mrn });

        b.Property(x => x.CreatedAt).IsRequired();
    }
}
