using HMS.Module.Admission.Features.Admission.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Admission.Infrastructure.Persistence.Configurations;

public sealed class WardRoomConfiguration : IEntityTypeConfiguration<myWardRoom>
{
    public void Configure(EntityTypeBuilder<myWardRoom> b)
    {
        b.ToTable("WardRooms");
        b.HasKey(x => x.WardRoomId);
        b.Property(x => x.WardRoomId).UseHiLo("HmsIdSeq");
        //b.HasKey(x => x.WardRoomId);
        //b.Property(x => x.WardRoomId)
        //    .ValueGeneratedOnAdd()
        //    .HasDefaultValueSql("NEXT VALUE FOR [dbo].[HmsIdSeq]");

        b.Property(x => x.RoomNumber).HasMaxLength(16).IsRequired();
        b.Property(x => x.Capacity).HasDefaultValue(1);

        b.HasIndex(x => new { x.WardId, x.RoomNumber }).IsUnique();

        // FK shadows (this module only)
        b.HasOne<myWard>().WithMany().HasForeignKey(x => x.WardId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne<myRoomType>().WithMany().HasForeignKey(x => x.RoomTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}
