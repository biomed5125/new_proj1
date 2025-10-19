// HMS.Communication/Infrastructure/Persistence/Configurations/CommMessageInboundConfiguration.cs
using HMS.Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Communication.Infrastructure.Persistence.Configurations;
public sealed class CommMessageInboundConfiguration : IEntityTypeConfiguration<CommMessageInbound>
{
    public void Configure(EntityTypeBuilder<CommMessageInbound> b)
    {
        b.ToTable("CommMessageInbound");
        b.HasKey(x => x.CommMessageInboundId);
        b.Property(x => x.CommMessageInboundId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Comm");

        b.Property(x => x.DeviceId).IsRequired();
        b.Property(x => x.At).IsRequired();
        b.Property(x => x.Direction).HasMaxLength(8);
        b.Property(x => x.Transport).HasMaxLength(40);
        b.Property(x => x.Ascii).HasMaxLength(8000);
        b.Property(x => x.BusinessNo).HasMaxLength(40); // no IsRequired()
    }
}
