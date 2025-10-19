// HMS.Communication/Infrastructure/Persistence/Configurations/CommDeadLetterResultConfiguration.cs
using HMS.Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Communication.Infrastructure.Persistence.Configurations;
public sealed class CommDeadLetterResultConfiguration : IEntityTypeConfiguration<DeadLetterResult>
{
    public void Configure(EntityTypeBuilder<DeadLetterResult> b)
    {
        b.ToTable("CommDeadLetters");
        b.HasKey(x => x.DeadLetterResultId);
        b.Property(x => x.DeadLetterResultId)
             .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Comm");

        b.Property(x => x.DeviceId).IsRequired();
        b.Property(x => x.Reason).HasMaxLength(200).IsRequired();
        b.Property(x => x.Payload).HasMaxLength(4000);
        b.Property(x => x.At).IsRequired();
    }
}
