// DeviceOutboxConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class DeviceOutboxConfiguration : IEntityTypeConfiguration<myDeviceOutbox>
    {
        public void Configure(EntityTypeBuilder<myDeviceOutbox> b)
        {
            b.ToTable("DeviceOutbox");
            b.HasKey(x => x.DeviceOutboxId);
            b.Property(x => x.DeviceOutboxId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            b.Property(x => x.DeviceName).HasMaxLength(50).IsRequired();
            b.Property(x => x.Payload).HasMaxLength(4000).IsRequired();
            // Sent is bool; no MaxLength
        }
    }

}
