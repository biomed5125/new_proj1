using HMS.Communication.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Communication.Infrastructure.Persistence.Configurations
{
    public sealed class AnalyzerProfileConfiguration : IEntityTypeConfiguration<AnalyzerProfile>
    {
        public void Configure(EntityTypeBuilder<AnalyzerProfile> b)
        {
            b.ToTable("AnalyzerProfiles");
            b.HasKey(x => x.AnalyzerProfileId);
            b.Property(x => x.AnalyzerProfileId)
                 .HasDefaultValueSql("NEXT VALUE FOR dbo.HmsIdSeq_Comm");

            b.Property(x => x.Name).HasMaxLength(80).IsRequired();
            b.Property(x => x.Protocol).HasMaxLength(40).IsRequired();
            b.Property(x => x.DriverClass).HasMaxLength(80).IsRequired();
            b.Property(x => x.PortSettings).HasMaxLength(256);
            b.Property(x => x.DefaultMode).HasMaxLength(40);
            b.Property(x => x.Notes).HasMaxLength(400);
        }
    }
}
