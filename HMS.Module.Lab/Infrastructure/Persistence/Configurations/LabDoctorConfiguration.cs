using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class LabDoctorConfiguration : IEntityTypeConfiguration<myLabDoctor>
    {
        public void Configure(EntityTypeBuilder<myLabDoctor> e)
        {

            e.ToTable("LabDoctors", "dbo");
            e.HasKey(x => x.LabDoctorId);
            // IDs from module sequence
            e.Property(x => x.LabDoctorId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Specialty).HasMaxLength(200).IsRequired();
            e.Property(x => x.LicenseNo).HasMaxLength(80);
            e.Property(x => x.Phone).HasMaxLength(40);
            e.Property(x => x.Email).HasMaxLength(120);
        }
    }
}
