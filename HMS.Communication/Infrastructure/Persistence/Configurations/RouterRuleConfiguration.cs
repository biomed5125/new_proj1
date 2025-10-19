using HMS.Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Communication.Infrastructure.Persistence.Configurations;
public class RouterRuleConfiguration : IEntityTypeConfiguration<RouterRule>
{
    public void Configure(EntityTypeBuilder<RouterRule> b)
    {
        b.ToTable("RouterRule");
        b.HasKey(x => x.Id);
        // ✅ PK comes from module sequence
        b.Property(x => x.Id)
         .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Comm");


        b.Property(x => x.Target).IsRequired().HasMaxLength(40);
        b.Property(x => x.RecordType).HasMaxLength(4);
        b.Property(x => x.TestCodeRegex).HasMaxLength(256);
        b.HasIndex(x => new { x.IsEnabled, x.Priority });
    }
}
