using HMS.Communication.Domain.CommEntities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Communication.Infrastructure.Persistence;

public sealed class CommMessageConfiguration :
    IEntityTypeConfiguration<Outbox>,
    IEntityTypeConfiguration<InboxFrame>,
    IEntityTypeConfiguration<InboxBatch>
{
    public void Configure(EntityTypeBuilder<Outbox> b)
    {
        b.ToTable("Outbox", "comm");
        b.HasKey(x => x.OutboxId);
        b.Property(x => x.AccessionNumber).HasMaxLength(32);
        b.Property(x => x.OrderNo).HasMaxLength(32);
        b.Property(x => x.Payload).HasColumnType("nvarchar(max)");
        b.HasIndex(x => new { x.DeviceId, x.State, x.CreatedAt });
    }

    public void Configure(EntityTypeBuilder<InboxFrame> b)
    {
        b.ToTable("InboxFrames", "comm");
        b.HasKey(x => x.InboxFrameId);
        b.Property(x => x.RawText).HasColumnType("nvarchar(max)");
        b.Property(x => x.Checksum).HasMaxLength(2);
        b.HasIndex(x => new { x.DeviceId, x.BatchId, x.FrameNo });
    }

    public void Configure(EntityTypeBuilder<InboxBatch> b)
    {
        b.ToTable("InboxBatches", "comm");
        b.HasKey(x => x.InboxBatchId);
        b.HasIndex(x => new { x.DeviceId, x.ReceivedAt });
    }
}
