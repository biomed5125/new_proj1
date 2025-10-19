// SpecimenTypeConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class SpecimenTypeConfiguration : IEntityTypeConfiguration<mySpecimenType>
    {
        public void Configure(EntityTypeBuilder<mySpecimenType> b)
        {
            b.ToTable("SpecimenTypes");

            b.HasKey(x => x.SpecimenTypeId);

            b.Property(x => x.SpecimenTypeId)
                .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            b.Property(x => x.Name)
                .HasMaxLength(80)
                .IsRequired();

            b.Property(x => x.Code)
                .HasMaxLength(20)
                .IsRequired(); // <- if you want Code optional, remove this

            // Auditing (optional but useful)
            b.Property(x => x.CreatedAt)
                .HasDefaultValueSql("SYSUTCDATETIME()");
            b.Property(x => x.CreatedBy).HasMaxLength(64);
            b.Property(x => x.UpdatedBy).HasMaxLength(64);

            // Uniqueness
            b.HasIndex(x => x.Name).IsUnique();

            // Unique on Code; if Code is nullable, keep the filter so multiple NULLs are allowed
            b.HasIndex(x => x.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL");
        }
    }

}
