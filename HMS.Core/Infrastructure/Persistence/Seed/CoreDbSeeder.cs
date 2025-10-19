// HMS.Core/Infrastructure/Persistence/Seed/CoreDbSeeder.cs
using HMS.Core.Infrastructure.Persistence;
using HMS.Core.ReadModels;
using HMS.Module.Admission.Infrastructure.Persistence;
using HMS.Module.Appointment.Infrastructure.Persistence;
using HMS.Module.Doctor.Infrastructure.Persistence;
using HMS.Module.Patient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Core.Infrastructure.Persistence.Seed;

public static class CoreDbSeeder
{
    public static async Task SeedFromModulesAsync(IServiceProvider sp, CancellationToken ct = default)
    {
        // Always create a FRESH scope here
        using var scope = sp.CreateScope();
        var provider = scope.ServiceProvider;

        var core = provider.GetRequiredService<CoreDbContext>();
        var pat = provider.GetService<PatientDbContext>();
        var appt = provider.GetService<AppointmentDbContext>();
        var adm = provider.GetService<AdmissionDbContext>();
        var doc = provider.GetService<DoctorDbContext>();

        await core.Database.MigrateAsync(ct);

        // ---------- Materialize snapshots (NO IQueryable returns) ----------
        var patients = pat is null
            ? new List<CorePatient>()
            : await pat.Patients
                .AsNoTracking()
                .Where(p => !p.IsDeleted)
                .Select(p => new CorePatient
                {
                    PatientId = p.PatientId,
                    Mrn = p.Mrn ?? "",
                    FirstName = p.FirstName ?? "",
                    LastName = p.LastName ?? "",
                    DateOfBirth = p.DateOfBirth,
                    Gender = p.Gender,
                    Phone = p.Phone,
                    Email = p.Email
                })
                .ToListAsync(ct);

        var doctors = doc is null
            ? new List<CoreDoctor>()
            : await doc.Doctors
                .AsNoTracking()
                .Where(d => !d.IsDeleted)
                .Select(d => new CoreDoctor
                {
                    DoctorId = d.DoctorId,
                    FirstName = d.FirstName ?? "",
                    LastName = d.LastName ?? "",
                    LicenseNumber = d.LicenseNumber ?? "",
                    Specialty = d.Specialty
                })
                .ToListAsync(ct);

        var admissions = adm is null
            ? new List<CoreAdmission>()
            : await adm.Admissions
                .AsNoTracking()
                .Where(a => !a.IsDeleted)
                .Select(a => new CoreAdmission
                {
                    AdmissionId = a.AdmissionId,
                    PatientId = a.PatientId,
                    DoctorId = a.DoctorId,
                    WardRoomId = (long)a.WardRoomId,
                    EncounterNo = a.EncounterNo ?? "",
                    AdmittedAtUtc = a.AdmittedAtUtc,
                    DischargedAtUtc = a.DischargedAtUtc,
                    Status = (int)a.Status,
                    DiagnosisOnAdmission = a.DiagnosisOnAdmission,
                    Notes = a.Notes
                })
                .ToListAsync(ct);

        var appointments = appt is null
            ? new List<CoreAppointment>()
            : await appt.Appointments
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Select(x => new CoreAppointment
                {
                    AppointmentId = x.AppointmentId,
                    AppointmentNo = x.AppointmentNo ?? "",
                    PatientId = x.PatientId,
                    DoctorId = (long)x.DoctorId,
                    ScheduledAtUtc = x.ScheduledAtUtc,
                    DurationMinutes = x.DurationMinutes,
                    Status = (int)x.Status,
                    Reason = x.Reason,
                    Notes = x.Notes
                })
                .ToListAsync(ct);

        // ---------- Simple idempotent upsert (demo) ----------
        // Patients
        foreach (var p in patients)
        {
            var exists = await core.Patients.AnyAsync(x => x.PatientId == p.PatientId, ct);
            if (!exists) core.Patients.Add(p);
        }

        // Doctors
        foreach (var d in doctors)
        {
            var exists = await core.Doctors.AnyAsync(x => x.DoctorId == d.DoctorId, ct);
            if (!exists) core.Doctors.Add(d);
        }

        // Admissions
        foreach (var a in admissions)
        {
            var exists = await core.Admissions.AnyAsync(x => x.AdmissionId == a.AdmissionId, ct);
            if (!exists) core.Admissions.Add(a);
        }

        // Appointments
        foreach (var a in appointments)
        {
            var exists = await core.Appointments.AnyAsync(x => x.AppointmentId == a.AppointmentId, ct);
            if (!exists) core.Appointments.Add(a);
        }

        await core.SaveChangesAsync(ct);
    }
}
