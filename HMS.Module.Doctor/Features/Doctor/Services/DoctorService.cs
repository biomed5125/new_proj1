// Features/Doctor/Services/DoctorService.cs
using HMS.Module.Doctor.Features.Doctor.Models.Dtos;
using HMS.Module.Doctor.Features.Doctor.Models.Entities;
using HMS.Module.Doctor.Features.Doctor.Queries;
using HMS.Module.Doctor.Features.Doctor.Repositories;
using HMS.SharedKernel.Results;

namespace HMS.Module.Doctor.Features.Doctor.Services;

public sealed class DoctorService : IDoctorService
{
    private readonly IDoctorReadRepo _read;
    private readonly IDoctorWriteRepo _write;

    public DoctorService(IDoctorReadRepo read, IDoctorWriteRepo write)
    { _read = read; _write = write; }

    private static DoctorDto ToDto(myDoctor d) => new(
        d.DoctorId, d.FirstName, d.LastName, $"{d.FirstName} {d.LastName}",
        d.LicenseNumber, d.Specialty, d.Phone, d.Email, d.IsActive, d.HireDateUtc
    );

    public async Task<List<DoctorDto>> ListAsync(DoctorQuery q, CancellationToken ct)
        => (await _read.ListAsync(q, ct)).Select(ToDto).ToList();

    public async Task<DoctorDto?> GetAsync(long id, CancellationToken ct)
        => (await _read.GetAsync(id, ct)) is { } d ? ToDto(d) : null;

    public async Task<Result<DoctorDto>> CreateAsync(CreateDoctorDto dto, string? user, CancellationToken ct)
    {
        if (await _read.LicenseExistsAsync(dto.LicenseNumber, null, ct))
            return Result<DoctorDto>.Fail("License number already exists.");

        var now = DateTime.UtcNow;
        var e = new myDoctor
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            LicenseNumber = dto.LicenseNumber,
            Specialty = dto.Specialty,
            Phone = dto.Phone,
            Email = dto.Email,
            IsActive = dto.IsActive,
            HireDateUtc = dto.HireDateUtc,
            CreatedAt = now,
            CreatedBy = user
        };

        await _write.AddAsync(e, ct);
        await _write.SaveAsync(ct);
        return Result<DoctorDto>.Success(ToDto(e));
    }

    public async Task<Result<DoctorDto>> UpdateAsync(long id, UpdateDoctorDto dto, string? user, CancellationToken ct)
    {
        var existing = await _read.GetAsync(id, ct);
        if (existing is null) return Result<DoctorDto>.Fail("Doctor not found.");

        // LicenseNumber is immutable by policy; ignore changes
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Specialty = dto.Specialty;
        existing.Phone = dto.Phone;
        existing.Email = dto.Email;
        existing.IsActive = dto.IsActive;
        existing.HireDateUtc = dto.HireDateUtc;
        existing.UpdatedAt = DateTime.UtcNow; existing.UpdatedBy = user;

        await _write.UpdateAsync(existing, ct);
        await _write.SaveAsync(ct);

        return Result<DoctorDto>.Success(ToDto(existing));
    }

    public async Task<bool> DeleteAsync(long id, string? user, CancellationToken ct)
    {
        var ok = await _write.SoftDeleteAsync(id, ct);
        if (ok) await _write.SaveAsync(ct);
        return ok;
    }
}
