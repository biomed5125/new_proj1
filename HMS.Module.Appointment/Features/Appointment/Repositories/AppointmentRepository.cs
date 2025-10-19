
//using HMS.Module.Appointment.Features.Appointment.Models.Entities;
//using HMS.Module.Appointment.Features.Appointment.Models.Enums;
//using HMS.Module.Appointment.Features.Appointment.Repositories;
//using HMS.Module.Appointment.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Api.Features.Appointment.Repositories;

//public class AppointmentRepository : IAppointmentRepository
//{
//    private readonly AppointmentDbContext _db;
//    public AppointmentRepository(AppointmentDbContext db) => _db = db;

//    public Task<myAppointment?> GetByIdAsync(long id, CancellationToken ct)
//        => _db.Set<myAppointment>().FirstOrDefaultAsync(x => x.AppointmentId == id && !x.IsDeleted, ct);

//    public async Task<List<myAppointment>> ListAsync(DateTime? fromUtc, DateTime? toUtc, long? patientId, long? doctorId, CancellationToken ct)
//    {
//        var q = _db.Set<myAppointment>().AsQueryable().Where(x => !x.IsDeleted);

//        if (fromUtc.HasValue) q = q.Where(x => x.ScheduledAtUtc >= fromUtc.Value);
//        if (toUtc.HasValue) q = q.Where(x => x.ScheduledAtUtc <= toUtc.Value);
//        if (patientId.HasValue) q = q.Where(x => x.PatientId == patientId.Value);
//        if (doctorId.HasValue) q = q.Where(x => x.DoctorId == doctorId.Value);

//        return await q.OrderBy(x => x.ScheduledAtUtc).ToListAsync(ct);
//    }

//    public async Task AddAsync(myAppointment entity, CancellationToken ct)
//    {
//        await _db.Set<myAppointment>().AddAsync(entity, ct);
//        await _db.SaveChangesAsync(ct);
//    }

//    public async Task UpdateAsync(myAppointment entity, CancellationToken ct)
//    {
//        _db.Set<myAppointment>().Update(entity);
//        await _db.SaveChangesAsync(ct);
//    }

//    public async Task SoftDeleteAsync(myAppointment entity, CancellationToken ct)
//    {
//        entity.IsDeleted = true;
//        _db.Set<myAppointment>().Update(entity);
//        await _db.SaveChangesAsync(ct);
//    }

//    // ===== Overlap checks =====

//    public async Task<bool> HasOverlapAsync(
//        long? doctorId, 
//        DateTime startUtc, 
//        int durationMinutes, 
//        long? excludeId, 
//        int bufferMinutes, 
//        CancellationToken ct)
//    {
//        if (doctorId is null) return false; // unassigned doctor → allow

//        var endUtc = startUtc.AddMinutes(durationMinutes);

//        // apply +/- buffer
//        var startWithBuffer = startUtc.AddMinutes(-bufferMinutes);
//        var endWithBuffer = endUtc.AddMinutes(+bufferMinutes);

//        return await _db.Set<myAppointment>()
//            .AsNoTracking()
//            .Where(a =>
//                !a.IsDeleted &&
//                a.DoctorId == doctorId &&
//                a.Status != AppointmentStatus.Cancelled && // ignore cancelled
//                (excludeId == null || a.AppointmentId != excludeId.Value) &&
//                a.ScheduledAtUtc < endWithBuffer &&                                  // existing.start < new.end(+buffer)
//                a.ScheduledAtUtc.AddMinutes(a.DurationMinutes) > startWithBuffer)    // existing.end   > new.start(-buffer)
//            .AnyAsync(ct);
//    }

//    public async Task<bool> HasPatientOverlapAsync(long patientId, DateTime startUtc, int durationMinutes, long? excludeId, int bufferMinutes, CancellationToken ct)
//    {
//        var endUtc = startUtc.AddMinutes(durationMinutes);
//        var startWithBuffer = startUtc.AddMinutes(-bufferMinutes);
//        var endWithBuffer = endUtc.AddMinutes(+bufferMinutes);

//        return await _db.Set<myAppointment>()
//            .AsNoTracking()
//            .Where(a =>
//                !a.IsDeleted &&
//                a.PatientId == patientId &&
//                a.Status != AppointmentStatus.Cancelled &&
//                (excludeId == null || a.AppointmentId != excludeId.Value) &&
//                a.ScheduledAtUtc < endWithBuffer &&
//                a.ScheduledAtUtc.AddMinutes(a.DurationMinutes) > startWithBuffer)
//            .AnyAsync(ct);
//    }
//}
