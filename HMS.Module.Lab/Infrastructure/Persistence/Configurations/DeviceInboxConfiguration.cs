// DeviceInboxConfiguration.cs
using HMS.Module.Lab.Features.Lab.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Lab.Infrastructure.Persistence.Configurations
{
    public sealed class DeviceInboxConfiguration : IEntityTypeConfiguration<myDeviceInbox>
    {
        public void Configure(EntityTypeBuilder<myDeviceInbox> b)
        {
            b.ToTable("DeviceInbox");
            b.HasKey(x => x.DeviceInboxId);
            b.Property(x => x.DeviceInboxId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Lab");

            b.Property(x => x.DeviceName).HasMaxLength(50).IsRequired();
            b.Property(x => x.Payload).HasMaxLength(4000);   // ASTM frames can be long
                                                             // bools don’t need HasMaxLength
        }
    }

}
