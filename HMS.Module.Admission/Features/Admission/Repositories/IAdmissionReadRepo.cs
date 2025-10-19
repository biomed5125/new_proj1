using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Features.Admission.Queries;

namespace HMS.Module.Admission.Features.Admission.Repositories;

public interface IAdmissionReadRepo
{
    Task<myAdmission?> GetAsync(long id, CancellationToken ct);
    Task<IReadOnlyList<myAdmission>> ListAsync(AdmissionQuery q, CancellationToken ct);
    Task<bool> HasOpenAsync(long patientId, CancellationToken ct);
}
