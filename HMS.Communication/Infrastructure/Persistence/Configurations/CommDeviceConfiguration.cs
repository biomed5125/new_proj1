// HMS.Communication/Infrastructure/Persistence/Configurations/CommDeviceConfiguration.cs
using HMS.Communication.Domain.Entities;
using HMS.Communication.Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Communication.Infrastructure.Persistence.Configurations;
public sealed class CommDeviceConfiguration : IEntityTypeConfiguration<CommDevice>
{
    public void Configure(EntityTypeBuilder<CommDevice> b)
    {
        b.ToTable("CommDevices");
        b.HasKey(x => x.CommDeviceId);
        b.Property(x => x.CommDeviceId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Comm");

        b.Property(x => x.DeviceCode).HasMaxLength(64).IsRequired();
        b.Property(x => x.Name).HasMaxLength(160).IsRequired();
        b.Property(x => x.PortName).HasMaxLength(160).IsRequired();
        b.Property(x => x.IsActive).HasDefaultValue(true);
        b.HasIndex(x => x.DeviceCode).IsUnique();

        b.HasOne(d => d.AnalyzerProfile)
        .WithMany(p => p.Devices)
        .HasForeignKey(d => d.AnalyzerProfileId)
        .OnDelete(DeleteBehavior.Restrict);


    }
}
