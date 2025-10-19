
//using HMS.Module.Appointment.Features.Appointment.Models.Entities;
//using HMS.Module.Appointment.Features.Appointment.Models.Enums;
//using HMS.Module.Appointment.Features.Appointment.Repositories;
//using HMS.Module.Appointment.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Module.Appointment.Features.Appointment.Queries;

//public class AppointmentReadRepo : IAppointmentReadRepo
//{
//    private readonly AppointmentDbContext _db;
//    public AppointmentReadRepo(AppointmentDbContext db) => _db = db;

//    public async Task<IReadOnlyList<ApptItem>> GetDoctorAppointmentsAsync(
//    long doctorId, DateTime s, DateTime e, CancellationToken ct)
//    {
//        return await _db.Set<myAppointment>()
//            .AsNoTracking()
//            .Where(a => !a.IsDeleted && a.DoctorId == doctorId
//                        && a.ScheduledAtUtc >= s && a.ScheduledAtUtc < e)
//            .OrderBy(a => a.ScheduledAtUtc)
//            .Select(a => new ApptItem
//            {
//                AppointmentId = a.AppointmentId,
//                AppointmentNo = a.AppointmentNo ?? "",
//                ScheduledAtUtc = a.ScheduledAtUtc,
//                DurationMinutes = a.DurationMinutes,
//                Status = a.Status.ToString(),
//                PatientId = a.PatientId,
//                PatientMrn = _db.Patients
//                                     .Where(p => p.PatientId == a.PatientId)
//                                     .Select(p => p.Mrn)
//                                     .FirstOrDefault() ?? "",
//                PatientName = _db.Patients
//                                     .Where(p => p.PatientId == a.PatientId)
//                                     .Select(p => p.FirstName + " " + p.LastName)
//                                     .FirstOrDefault() ?? "",
//                Reason = a.Reason ?? ""
//            })
//            .ToListAsync(ct);
//    }

//    public async Task<TodayBlock> GetDoctorApptKpisAsync(
//        long doctorId, DateTime s, DateTime e, CancellationToken ct)
//    {
//        var baseQ = _db.Set<myAppointment>().AsNoTracking()
//            .Where(a => !a.IsDeleted && a.DoctorId == doctorId
//                        && a.ScheduledAtUtc >= s && a.ScheduledAtUtc < e);

//        var total = await baseQ.CountAsync(ct);
//        var checkedIn = await baseQ.CountAsync(a => a.Status == AppointmentStatus.CheckedIn, ct);
//        var completed = await baseQ.CountAsync(a => a.Status == AppointmentStatus.Completed, ct);
//        var cancelled = await baseQ.CountAsync(a => a.Status == AppointmentStatus.Cancelled, ct);

//        return new TodayBlock
//        {
//            TotalAppointments = total,
//            CheckedIn = checkedIn,
//            Completed = completed,
//            Cancelled = cancelled
//        };
//    }
//}
