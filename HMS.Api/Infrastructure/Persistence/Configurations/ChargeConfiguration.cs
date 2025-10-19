//using HMS.Api.Features.Billing.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace HMS.Api.Infrastructure.Persistence.Configurations;

//public class ChargeConfiguration : IEntityTypeConfiguration<myCharge>
//{
//    public void Configure(EntityTypeBuilder<myCharge> b)
//    {
//        b.ToTable("Charges");
//        b.HasKey(x => x.ChargeId);
//        b.Property(x => x.ChargeId).UseHiLo("HmsIdSeq");

//        b.Property(x => x.Source).HasConversion<int>().IsRequired();
//        b.Property(x => x.Status).HasConversion<int>().IsRequired();

//        b.Property(x => x.Quantity).HasColumnType("decimal(18,2)").HasDefaultValue(1);
//        b.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
//        b.Property(x => x.Total).HasColumnType("decimal(18,2)");

//        // Bill/InvoiceConfiguration (if present)
//        b.Property(x => x.InvoiceNo).HasMaxLength(32);
//        b.HasIndex(x => x.InvoiceNo).IsUnique()
//         .HasFilter("[InvoiceNo] IS NOT NULL");

//        b.HasIndex(x => new { x.PatientId, x.AdmissionId });
//        // with this (one charge per order *per service*):
//        b.HasIndex(x => new { x.Source, x.SourceId, x.ServiceId }).IsUnique();
//    }
//}
