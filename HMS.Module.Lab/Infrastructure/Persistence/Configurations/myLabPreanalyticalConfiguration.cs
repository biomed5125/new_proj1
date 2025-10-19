// Infrastructure/Persistence/Configurations/myLabPreanalyticalConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class myLabPreanalyticalConfiguration : IEntityTypeConfiguration<myLabPreanalytical>
{
    public void Configure(EntityTypeBuilder<myLabPreanalytical> b)
    {
        b.ToTable("LabPreanalyticals");
        b.HasKey(x => x.LabPreanalyticalId);

        b.Property(x => x.LabPreanalyticalId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        b.Property(x => x.ThyroidStatus).HasMaxLength(10);
        b.Property(x => x.AllergyNotes).HasMaxLength(400);
        b.Property(x => x.Notes).HasMaxLength(800);

        // 1:1 with myLabRequest
        b.HasOne(x => x.LabRequest)
         .WithOne()                               // no back-collection needed
         .HasForeignKey<myLabPreanalytical>(x => x.LabRequestId)
         .OnDelete(DeleteBehavior.Cascade)
         .IsRequired(false);

        b.HasIndex(x => x.LabRequestId).IsUnique();

        // Auditing (optional but useful)
        b.Property(x => x.CreatedAt)
            .HasDefaultValueSql("SYSUTCDATETIME()");
        b.Property(x => x.CreatedBy).HasMaxLength(64);
    }
}
