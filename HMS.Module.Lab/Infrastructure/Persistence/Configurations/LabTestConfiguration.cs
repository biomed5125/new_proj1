using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;
public sealed class LabTestConfiguration : IEntityTypeConfiguration<myLabTest>
{
    public void Configure(EntityTypeBuilder<myLabTest> b)
    {
        b.ToTable("LabTests");
        b.HasKey(x => x.LabTestId);

        b.Property(x => x.LabTestId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        b.Property(x => x.Code).HasMaxLength(40).IsRequired();
        b.Property(x => x.Name).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();

        // NEW reference range columns
        b.Property(x => x.RefLow).HasColumnType("decimal(18,4)")  // 4 dp usually enough; adjust as needed
            .IsRequired(false);

        b.Property(x => x.RefHigh).HasColumnType("decimal(18,4)").IsRequired(false);

        b.Property(x => x.Unit).HasMaxLength(32);
        b.Property(x => x.Price).HasPrecision(10, 2);
        b.Property(x => x.TatMinutes).IsRequired();

        b.HasOne(x => x.DefaultReferenceRange)
         .WithMany()
         .HasForeignKey(x => x.DefaultReferenceRangeId)
         .OnDelete(DeleteBehavior.SetNull);

        b.HasOne(x => x.SpecimenType)
         .WithMany()
         .HasForeignKey(x => x.SpecimenTypeId)
         .OnDelete(DeleteBehavior.Restrict);

        // Soft-delete
        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
