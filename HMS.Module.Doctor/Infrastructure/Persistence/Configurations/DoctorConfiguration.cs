// Infrastructure/Persistence/Configurations/DoctorConfiguration.cs
using HMS.Module.Doctor.Features.Doctor.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace HMS.Module.Doctor.Infrastructure.Persistence.Configurations;

public sealed class DoctorConfiguration : IEntityTypeConfiguration<myDoctor>
{
    public void Configure(EntityTypeBuilder<myDoctor> b)
    {
        b.ToTable("Doctors");
        b.HasKey(x => x.DoctorId);
        b.Property(x => x.DoctorId).UseHiLo("HmsIdSeq");
        //b.HasKey(x => x.DoctorId);
        //b.Property(x => x.DoctorId)
        //.ValueGeneratedOnAdd()
        //.HasDefaultValueSql("NEXT VALUE FOR [dbo].[HmsIdSeq]");
        //b.Property(x => x.DoctorId).UseHiLo("HmsIdSeq");

        b.Property(x => x.FirstName).IsRequired().HasMaxLength(60);
        b.Property(x => x.LastName).IsRequired().HasMaxLength(60);
        b.Property(x => x.LicenseNumber).IsRequired().HasMaxLength(40);
        b.Property(x => x.Specialty).HasMaxLength(80);
        b.Property(x => x.Phone).HasMaxLength(30);
        b.Property(x => x.Email).HasMaxLength(120);

        b.HasIndex(x => x.LicenseNumber).IsUnique();
        b.HasIndex(x => new { x.LastName, x.FirstName });
    }
}
