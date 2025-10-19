//using HMS.Module.Appointment.Features.Appointment.Models.Entities;

//namespace HMS.Module.Appointment.Features.Appointment.Repositories;

//public interface IAppointmentRepository
//{
//    Task<myAppointment?> GetByIdAsync(long id, CancellationToken ct);
//    Task<List<myAppointment>> ListAsync(DateTime? fromUtc, DateTime? toUtc, long? patientId, long? doctorId, CancellationToken ct);
//    Task AddAsync(myAppointment entity, CancellationToken ct);
//    Task UpdateAsync(myAppointment entity, CancellationToken ct);
//    Task SoftDeleteAsync(myAppointment entity, CancellationToken ct);
//    // Doctor overlap (with buffer minutes)
//    Task<bool> HasOverlapAsync(long? doctorId, DateTime startUtc, int durationMinutes, long? excludeId, int bufferMinutes, CancellationToken ct);

//    // NEW: Patient overlap (with buffer minutes)
//    Task<bool> HasPatientOverlapAsync(long patientId, DateTime startUtc, int durationMinutes, long? excludeId, int bufferMinutes, CancellationToken ct);
//}

