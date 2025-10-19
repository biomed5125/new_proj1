using HMS.Module.Appointment.Features.Appointment.Models.Entities;
using HMS.Module.Appointment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Appointment.Features.Appointment.Repositories;

public class AppointmentReadRepo : IAppointmentReadRepo
{
    private readonly AppointmentDbContext _db;
    public AppointmentReadRepo(AppointmentDbContext db) => _db = db;

    public Task<myAppointment?> GetAsync(long id, CancellationToken ct)
        => _db.Appointments.AsNoTracking().FirstOrDefaultAsync(x => x.AppointmentId == id, ct);

    public async Task<List<myAppointment>> ListAsync(DateTime? fromUtc, DateTime? toUtc, long? patientId, long? doctorId, CancellationToken ct)
    {
        var q = _db.Appointments.AsNoTracking().AsQueryable();
        if (fromUtc.HasValue) q = q.Where(x => x.ScheduledAtUtc >= fromUtc.Value);
        if (toUtc.HasValue) q = q.Where(x => x.ScheduledAtUtc < toUtc.Value);
        if (patientId != null) q = q.Where(x => x.PatientId == patientId);
        if (doctorId != null) q = q.Where(x => x.DoctorId == doctorId);
        return await q.OrderBy(x => x.ScheduledAtUtc).ToListAsync(ct);
    }
}

