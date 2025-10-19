using HMS.Module.Admission.Features.Admission.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Admission.Infrastructure.Persistence.Configurations;

public sealed class RoomTypeConfiguration : IEntityTypeConfiguration<myRoomType>
{
    public void Configure(EntityTypeBuilder<myRoomType> b)
    {
        b.ToTable("RoomTypes");

        b.HasKey(x => x.RoomTypeId);
        b.Property(x => x.RoomTypeId).UseHiLo("HmsIdSeq");
        //b.HasKey(x => x.RoomTypeId);
        //b.Property(x => x.RoomTypeId)
        //    .ValueGeneratedOnAdd()
        //    .HasDefaultValueSql("NEXT VALUE FOR [dbo].[HmsIdSeq]");
        b.Property(x => x.Name).HasMaxLength(64).IsRequired();
        b.Property(x => x.DailyRate).HasPrecision(10, 2);
        b.HasIndex(x => x.Name).IsUnique();
    }
}
