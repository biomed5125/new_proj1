
using HMS.Module.Patient.Features.Patient.Models.Dtos;
using HMS.Module.Patient.Features.Patient.Queries;
using HMS.SharedKernel;

namespace HMS.Module.Patient.Features.Patient.Repositories
{
    public interface IPatientReadRepo
    {
        Task<PatientDto?> GetAsync(long id, CancellationToken ct);
        Task<IReadOnlyList<PatientDto>> ListAsync(PatientQuery q, CancellationToken ct);
    }

}
