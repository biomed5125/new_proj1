using HMS.Module.Patient.Features.Patient.Models.Dtos;
using HMS.Module.Patient.Features.Patient.Models.Entities;
using HMS.Module.Patient.Features.Patient.Repositories;
using HMS.Module.Patient.Features.Patient.Queries;
using HMS.SharedKernel.Results;
using HMS.SharedKernel.Ids;

namespace HMS.Module.Patient.Features.Patient.Services;

public class PatientService : IPatientService
{
    private readonly IPatientReadRepo _read;
    private readonly IPatientWriteRepo _write;
    private readonly IBusinessIdGenerator _ids;     // <-- inject

    public PatientService(IPatientReadRepo read, IPatientWriteRepo write, IBusinessIdGenerator ids)
    {
        _read = read;
        _write = write;
        _ids = ids;
    }

    public async Task<Result<PatientDto>> CreateAsync(CreatePatientDto dto, string? user, CancellationToken ct)
    {
        var mrn = string.IsNullOrWhiteSpace(dto.Mrn) ? _ids.NewMrn() : dto.Mrn.Trim();

        if (await _write.MrnExistsAsync(mrn, ct))
            return Result<PatientDto>.Fail("MRN already exists.");

        var now = DateTime.UtcNow;
        var e = new myPatient
        {
            Mrn = mrn,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Email = dto.Email,
            CreatedAt = now,
            CreatedBy = user
        };

        await _write.AddAsync(e, ct);
        var created = await _read.GetAsync(e.PatientId, ct);
        return created is null ? Result<PatientDto>.Fail("Create failed.") : Result<PatientDto>.Success(created);
    }

    public async Task<Result<PatientDto>> UpdateAsync(long id, UpdatePatientDto dto, string? user, CancellationToken ct)
    {
        var existing = await _read.GetAsync(id, ct);
        if (existing is null)
            return Result<PatientDto>.Fail("Patient not found.");

        var e = new myPatient
        {
            PatientId = id,
            // preserve MRN (not editable here)
            Mrn = existing.Mrn,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Email = dto.Email,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = user
        };

        await _write.UpdateAsync(e, ct);

        var updated = await _read.GetAsync(id, ct);
        return updated is null
            ? Result<PatientDto>.Fail("Update failed.")
            : Result<PatientDto>.Success(updated);
    }

    public async Task<Result> DeleteAsync(long id, string? user, CancellationToken ct)
    {
        await _write.SoftDeleteAsync(id, ct);
        return Result.Success();
    }

    public Task<PatientDto?> GetAsync(long id, CancellationToken ct)
        => _read.GetAsync(id, ct);

    public async Task<List<PatientDto>> ListAsync(CancellationToken ct)
     => (await _read.ListAsync(new PatientQuery(), ct)).ToList();

    public async Task<List<PatientDto>> ListAsync(PatientQuery q, CancellationToken ct)
    => (await _read.ListAsync(q, ct)).ToList();
}
