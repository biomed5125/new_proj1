using HMS.Module.Admission.Features.Admission.Models.Dtos;
using HMS.Module.Admission.Features.Admission.Queries;
using HMS.SharedKernel.Results;

namespace HMS.Module.Admission.Features.Admission.Services;

public interface IAdmissionService
{
    Task<AdmissionDto?> GetAsync(long id, CancellationToken ct);
    Task<List<AdmissionDto>> ListAsync(AdmissionQuery q, CancellationToken ct);
    Task<Result<AdmissionDto>> CreateAsync(CreateAdmissionDto dto, string? user, CancellationToken ct);
    Task<Result<AdmissionDto>> UpdateAsync(long id, UpdateAdmissionDto dto, string? user, CancellationToken ct);
    Task<Result> DischargeAsync(long id, DischargeAdmissionDto dto, string? user, CancellationToken ct);
    Task<Result> DeleteAsync(long id, string? user, CancellationToken ct);


}
