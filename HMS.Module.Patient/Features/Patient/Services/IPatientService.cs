using HMS.Module.Patient.Features.Patient.Models.Dtos;
using HMS.Module.Patient.Features.Patient.Queries;
using HMS.SharedKernel.Results;

namespace HMS.Module.Patient.Features.Patient.Services
{
    public interface IPatientService
    {
        Task<List<PatientDto>> ListAsync(PatientQuery q, CancellationToken ct);
        Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto, string? user, CancellationToken ct);
        Task<Result<PatientDto>> UpdateAsync(long id, UpdatePatientDto dto, string? user, CancellationToken ct);
        Task<Result> DeleteAsync(long id, string? user, CancellationToken ct);
        Task<PatientDto?> GetAsync(long id, CancellationToken ct);
        Task<List<PatientDto>> ListAsync(CancellationToken ct);
    }
}