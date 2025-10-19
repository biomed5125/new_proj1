// HMS.Communication/Infrastructure/Persistence/Configurations/CommMessageOutboundConfiguration.cs
using HMS.Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Communication.Infrastructure.Persistence.Configurations;
public sealed class CommMessageOutboundConfiguration : IEntityTypeConfiguration<CommMessageOutbound>
{
    public void Configure(EntityTypeBuilder<CommMessageOutbound> b)
    {
        b.ToTable("CommMessageOutbound");
        b.HasKey(x => x.CommMessageOutboundId);
        b.Property(x => x.CommMessageOutboundId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Comm");

        b.Property(x => x.DeviceId).IsRequired();
        b.Property(x => x.At).IsRequired();
        b.Property(x => x.Transport).HasMaxLength(40);
        b.Property(x => x.Payload).HasMaxLength(4000);
        b.Property(x => x.BusinessNo).HasMaxLength(40); // no IsRequired()
    }
}
