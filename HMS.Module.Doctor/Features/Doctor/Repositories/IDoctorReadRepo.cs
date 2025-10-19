// Features/Doctor/Repositories/IDoctorReadRepo.cs
using HMS.Module.Doctor.Features.Doctor.Models.Entities;
using HMS.Module.Doctor.Features.Doctor.Queries;

namespace HMS.Module.Doctor.Features.Doctor.Repositories;

public interface IDoctorReadRepo
{
    Task<myDoctor?> GetAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<myDoctor>> ListAsync(DoctorQuery q, CancellationToken ct);
    Task<bool> LicenseExistsAsync(string license, long? excludeId, CancellationToken ct);
}
