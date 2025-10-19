

using HMS.Module.Patient.Features.Patient.Models.Entities;

namespace HMS.Module.Patient.Features.Patient.Repositories
{
    public interface IPatientWriteRepo
    {
        Task<bool> MrnExistsAsync(string mrn, CancellationToken ct);
        Task AddAsync(myPatient e, CancellationToken ct);
        Task UpdateAsync(myPatient e, CancellationToken ct);
        Task SoftDeleteAsync(long id, CancellationToken ct);
    }
}
