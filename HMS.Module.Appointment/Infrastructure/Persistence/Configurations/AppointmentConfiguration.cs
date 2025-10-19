using HMS.Module.Appointment.Features.Appointment.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HMS.Module.Appointment.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<myAppointment>
{
    public void Configure(EntityTypeBuilder<myAppointment> b)
    {
        b.ToTable("Appointments");
        b.HasKey(x => x.AppointmentId);
        b.Property(x => x.AppointmentId).UseHiLo("HmsIdSeq");

        //b.HasKey(x => x.AppointmentId);
        //b.Property(x => x.AppointmentId)
        //.ValueGeneratedOnAdd()
        //.HasDefaultValueSql("NEXT VALUE FOR [dbo].[HmsIdSeq]");

        b.Property(x => x.ScheduledAtUtc).IsRequired();
        b.Property(x => x.DurationMinutes).IsRequired();
        b.Property(x => x.Status).HasConversion<int>().IsRequired();

        b.Property(x => x.Reason).HasMaxLength(200);
        b.Property(x => x.Notes).HasMaxLength(1000);

        b.Property(x => x.AppointmentNo).HasMaxLength(32);
        b.HasIndex(x => x.AppointmentNo).IsUnique().HasFilter("[AppointmentNo] IS NOT NULL");

        b.HasIndex(x => new { x.PatientId, x.ScheduledAtUtc });
        b.HasIndex(x => new { x.DoctorId, x.ScheduledAtUtc });
    }
}
