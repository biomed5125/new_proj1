//using HMS.Api.Features.Billing.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace HMS.Api.Infrastructure.Persistence.Configurations;

//public class HospitalServiceConfiguration : IEntityTypeConfiguration<myHospitalService>
//{
//    public void Configure(EntityTypeBuilder<myHospitalService> b)
//    {
//        b.ToTable("HospitalServices");
//        b.HasKey(x => x.ServiceId);
//        b.Property(x => x.ServiceId).UseHiLo("HmsIdSeq");

//        b.Property(x => x.Code).HasMaxLength(40).IsRequired();
//        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
//        b.Property(x => x.Unit).HasMaxLength(20).IsRequired();
//        b.Property(x => x.Category).HasConversion<int>().IsRequired();
//        b.Property(x => x.DefaultPrice).HasColumnType("decimal(18,2)").IsRequired();

//        b.HasIndex(x => x.Code).IsUnique();
//    }
//}
