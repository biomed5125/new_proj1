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
    public sealed class LabPatientConfiguration : IEntityTypeConfiguration<myLabPatient>
    {
        public void Configure(EntityTypeBuilder<myLabPatient> e)
        {

            e.ToTable("LabPatients", "dbo");
            e.HasKey(x => x.LabPatientId);
            // IDs from module sequence
            e.Property(x => x.LabPatientId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            e.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            e.Property(x => x.Mrn).HasMaxLength(40);
            e.Property(x => x.Sex).HasMaxLength(1);
            e.Property(x => x.Phone).HasMaxLength(40);
            e.Property(x => x.Email).HasMaxLength(120);
            e.Property(x => x.Address).HasMaxLength(400);
        }
    }

}
