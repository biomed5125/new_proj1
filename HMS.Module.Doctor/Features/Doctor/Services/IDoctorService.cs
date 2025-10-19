// Features/Doctor/Services/IDoctorService.cs
using HMS.Module.Doctor.Features.Doctor.Models.Dtos;
using HMS.Module.Doctor.Features.Doctor.Queries;
using HMS.SharedKernel.Results;

namespace HMS.Module.Doctor.Features.Doctor.Services;

public interface IDoctorService
{
    Task<List<DoctorDto>> ListAsync(DoctorQuery q, CancellationToken ct);
    Task<DoctorDto?> GetAsync(long id, CancellationToken ct);
    Task<Result<DoctorDto>> CreateAsync(CreateDoctorDto dto, string? user, CancellationToken ct);
    Task<Result<DoctorDto>> UpdateAsync(long id, UpdateDoctorDto dto, string? user, CancellationToken ct);
    Task<bool> DeleteAsync(long id, string? user, CancellationToken ct);
}
