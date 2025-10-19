using HMS.Api.Features.Admission.Models.Entities;
using HMS.Api.Features.Admission.Models.Enums;

namespace HMS.Api.Features.Admission.Repositories;

public interface IAdmissionRepository
{
    Task<myAdmission?> GetByIdAsync(long id, CancellationToken ct);
    Task<List<myAdmission>> ListAsync(long? patientId, long? wardId, long? wardRoomId, AdmissionStatus? status, CancellationToken ct);
    Task AddAsync(myAdmission entity, CancellationToken ct);
    Task UpdateAsync(myAdmission entity, CancellationToken ct);

    Task<bool> PatientHasOpenAdmissionAsync(long patientId, long? excludeAdmissionId, CancellationToken ct);
    Task<(int capacity, int occupied)> GetRoomOccupancyAsync(long wardRoomId, CancellationToken ct);
    Task<bool> WardRoomExistsAsync(long wardRoomId, CancellationToken ct);
    Task<bool> PatientExistsAsync(long patientId, CancellationToken ct);
}
