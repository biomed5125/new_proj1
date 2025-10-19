using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;
public sealed class LabPanelConfiguration : IEntityTypeConfiguration<myLabPanel>
{
    public void Configure(EntityTypeBuilder<myLabPanel> b)
    {
        b.ToTable("LabPanels");
        b.HasKey(x => x.LabPanelId);
        b.Property(x => x.LabPanelId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");
        b.Property(x => x.Code).HasMaxLength(40).IsRequired();
        b.Property(x => x.Name).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
    }
}
