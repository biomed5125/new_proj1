//using HMS.Api.Features.MedicalAction.Models.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace HMS.Api.Infrastructure.Persistence.Configurations;

//public class MedicalActionConfiguration : IEntityTypeConfiguration<myMedicalAction>
//{
//    public void Configure(EntityTypeBuilder<myMedicalAction> b)
//    {
//        b.ToTable("MedicalActions");
//        b.HasKey(x => x.MedicalActionId);
//        b.Property(x => x.MedicalActionId).UseHiLo("HmsIdSeq");
//        b.Property(x => x.ActionType).HasConversion<int>().IsRequired();
//        b.Property(x => x.Status).HasConversion<int>().IsRequired();
//        b.Property(x => x.Notes).HasMaxLength(1000);
//        b.HasIndex(x => new { x.PatientId, x.ActionType, x.Status });
//    }
//}
