using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations;
public sealed class LabPanelItemConfiguration : IEntityTypeConfiguration<myLabPanelItem>
{
    public void Configure(EntityTypeBuilder<myLabPanelItem> b)
    {
        b.ToTable("LabPanelItems");
        b.HasKey(x => x.LabPanelItemId);
        b.Property(x => x.LabPanelItemId)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

        b.HasOne(x => x.Panel).WithMany(x => x.Items).HasForeignKey(x => x.LabPanelId);
        b.HasOne(x => x.Test).WithMany().HasForeignKey(x => x.LabTestId);
        b.Property(x => x.SortOrder).HasDefaultValue(0);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
