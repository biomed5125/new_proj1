using HMS.Module.Admission.Features.Admission.Models.Dtos;
using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Features.Admission.Models.Enums;
using HMS.Module.Admission.Features.Admission.Queries;
using HMS.Module.Admission.Features.Admission.Repositories;
using HMS.SharedKernel.Ids;
using HMS.SharedKernel.Results;

namespace HMS.Module.Admission.Features.Admission.Services;

public sealed class AdmissionService : IAdmissionService
{
    private readonly IAdmissionReadRepo _read;
    private readonly IAdmissionWriteRepo _write;
    private readonly IBusinessIdGenerator _ids;

    public AdmissionService(IAdmissionReadRepo read, IAdmissionWriteRepo write, IBusinessIdGenerator ids)
    { _read = read; _write = write; _ids = ids; }

    private static AdmissionDto ToDto(myAdmission a) => new(
        a.AdmissionId, a.PatientId, a.DoctorId, a.WardRoomId, a.EncounterNo,
        a.AdmittedAtUtc, a.DischargedAtUtc, (int)a.Status, a.DiagnosisOnAdmission, a.Notes);

    public async Task<AdmissionDto?> GetAsync(long id, CancellationToken ct)
        => (await _read.GetAsync(id, ct)) is { } e ? ToDto(e) : null;

    public async Task<List<AdmissionDto>> ListAsync(AdmissionQuery q, CancellationToken ct)
        => (await _read.ListAsync(q, ct)).Select(ToDto).ToList();

    public async Task<Result<AdmissionDto>> CreateAsync(CreateAdmissionDto dto, string? user, CancellationToken ct)
    {
        if (dto.PatientId <= 0) return Result<AdmissionDto>.Fail("Invalid patient.");
        if (await _read.HasOpenAsync(dto.PatientId, ct)) return Result<AdmissionDto>.Fail("Patient already admitted.");

        var e = new myAdmission
        {
            PatientId = dto.PatientId,
            DoctorId = dto.DoctorId,
            WardRoomId = dto.WardRoomId,
            AdmittedAtUtc = dto.AdmittedAtUtc,
            DiagnosisOnAdmission = dto.DiagnosisOnAdmission,
            Notes = dto.Notes,
            Status = AdmissionStatus.Admitted,
            EncounterNo = _ids.NewEncounterNo(dto.AdmittedAtUtc),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user
        };

        await _write.AddAsync(e, ct);
        await _write.SaveAsync(ct);
        return Result<AdmissionDto>.Success(ToDto(e));
    }

    public async Task<Result<AdmissionDto>> UpdateAsync(long id, UpdateAdmissionDto dto, string? user, CancellationToken ct)
    {
        var e = await _read.GetAsync(id, ct);
        if (e is null) return Result<AdmissionDto>.Fail("Not found.");
        if (e.Status == AdmissionStatus.Discharged) return Result<AdmissionDto>.Fail("Admission already discharged.");

        e.DoctorId = dto.DoctorId ?? e.DoctorId;
        e.WardRoomId = dto.WardRoomId ?? e.WardRoomId;
        e.DiagnosisOnAdmission = dto.DiagnosisOnAdmission ?? e.DiagnosisOnAdmission;
        e.Notes = dto.Notes ?? e.Notes;
        if (dto.Status is { } s && s == (int)AdmissionStatus.Cancelled) e.Status = AdmissionStatus.Cancelled;

        e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = user;

        await _write.UpdateAsync(e, ct);
        await _write.SaveAsync(ct);
        return Result<AdmissionDto>.Success(ToDto(e));
    }

    public async Task<Result> DischargeAsync(long id, DischargeAdmissionDto dto, string? user, CancellationToken ct)
    {
        var e = await _read.GetAsync(id, ct);
        if (e is null) return Result.Fail("Not found.");
        if (e.Status == AdmissionStatus.Discharged) return Result.Success(); // idempotent

        e.DischargedAtUtc = dto.DischargedAtUtc;
        e.Notes = string.IsNullOrWhiteSpace(dto.Notes) ? e.Notes : dto.Notes;
        e.Status = AdmissionStatus.Discharged;
        e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = user;

        await _write.UpdateAsync(e, ct);
        await _write.SaveAsync(ct);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(long id, string? user, CancellationToken ct)
    {
        await _write.SoftDeleteAsync(id, ct);
        await _write.SaveAsync(ct);
        return Result.Success();
    }
}
