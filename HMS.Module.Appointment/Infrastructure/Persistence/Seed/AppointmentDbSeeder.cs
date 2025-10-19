using HMS.Module.Appointment.Infrastructure.Persistence;
using HMS.Module.Appointment.Features.Appointment.Models.Entities;
using HMS.Module.Appointment.Features.Appointment.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using HMS.Module.Patient.Infrastructure.Persistence;

namespace HMS.Module.Appointment.Infrastructure.Persistence.Seed;

public static class AppointmentDbSeeder
{
    public static async Task SeedAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("AppointmentDbSeeder");
        var apptDb = sp.GetRequiredService<AppointmentDbContext>();

        await apptDb.Database.MigrateAsync(ct);

        if (await apptDb.Appointments.AnyAsync(ct))
        {
            logger.LogInformation("Appointment DB already has data. Skipping seed.");
            return;
        }

        // Try to pick a couple of patient ids from the Patient DB (if available)
        long p1 = 0, p2 = 0;
        var patientDb = sp.GetService<PatientDbContext>();
        if (patientDb is not null)
        {
            var ids = await patientDb.Patients
                .OrderBy(p => p.PatientId)
                .Select(p => p.PatientId)
                .Take(2)
                .ToListAsync(ct);

            if (ids.Count > 0) p1 = ids[0];
            if (ids.Count > 1) p2 = ids[1];
            if (p2 == 0) p2 = p1;
        }

        var now = DateTime.UtcNow;
        var tomorrow = DateTime.UtcNow.Date.AddDays(1);

        string NewApptNo(DateTime when) => $"AP{when:yyyyMMddHHmmssfff}";

        var appts = new[]
        {
            new myAppointment
            {
                PatientId = p1 == 0 ? 900001 : p1, // fallback id if patient DB not available
                DoctorId = 2001,
                ScheduledAtUtc = tomorrow.AddHours(9),
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled,
                Reason = "Initial consult",
                Notes  = "Seed data",
                AppointmentNo = NewApptNo(now),
                CreatedAt = now, CreatedBy = "seed"
            },
            new myAppointment
            {
                PatientId = p2 == 0 ? 900002 : p2,
                DoctorId = 2002,
                ScheduledAtUtc = tomorrow.AddHours(10),
                DurationMinutes = 30,
                Status = AppointmentStatus.Scheduled,
                Reason = "Follow-up",
                Notes  = "Seed data",
                AppointmentNo = NewApptNo(now.AddSeconds(1)),
                CreatedAt = now, CreatedBy = "seed"
            }
        };

        apptDb.Appointments.AddRange(appts);
        await apptDb.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} appointments.", appts.Length);
    }
}
