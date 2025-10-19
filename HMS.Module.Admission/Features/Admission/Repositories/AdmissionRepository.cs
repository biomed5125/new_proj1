using HMS.Api.Features.Admission.Models.Entities;
using HMS.Api.Features.Admission.Models.Enums;
using HMS.Api.Features.Billing.Models.Entities;
using HMS.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Features.Admission.Repositories;

public class AdmissionRepository : IAdmissionRepository
{
    private readonly HmsDbContext _db;
    public AdmissionRepository(HmsDbContext db) => _db = db;

    public Task<myAdmission?> GetByIdAsync(long id, CancellationToken ct)
        => _db.Set<myAdmission>().FirstOrDefaultAsync(a => a.AdmissionId == id && !a.IsDeleted, ct);

    public async Task<List<myAdmission>> ListAsync(long? patientId, long? wardId, long? wardRoomId, AdmissionStatus? status, CancellationToken ct)
    {
        var q = _db.Set<myAdmission>().AsQueryable().Where(a => !a.IsDeleted);

        if (patientId.HasValue) q = q.Where(a => a.PatientId == patientId.Value);
        if (wardRoomId.HasValue) q = q.Where(a => a.WardRoomId == wardRoomId.Value);
        if (wardId.HasValue)
        {
            var roomIds = await _db.Set<myWardRoom>()
                .Where(r => r.WardId == wardId.Value)
                .Select(r => r.WardRoomId)
                .ToListAsync(ct);
            q = q.Where(a => roomIds.Contains(a.WardRoomId));
        }
        if (status.HasValue) q = q.Where(a => a.Status == status.Value);

        return await q.OrderByDescending(a => a.AdmittedAtUtc).ToListAsync(ct);
    }

    public async Task AddAsync(myAdmission entity, CancellationToken ct)
    {
        await _db.Set<myAdmission>().AddAsync(entity, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(myAdmission entity, CancellationToken ct)
    {
        _db.Set<myAdmission>().Update(entity);
        await _db.SaveChangesAsync(ct);
    }

    public Task<bool> PatientHasOpenAdmissionAsync(long patientId, long? excludeAdmissionId, CancellationToken ct)
    {
        var q = _db.Set<myAdmission>().Where(a =>
            !a.IsDeleted &&
            a.PatientId == patientId &&
            a.Status == AdmissionStatus.Admitted &&
            a.DischargedAtUtc == null);

        if (excludeAdmissionId.HasValue) q = q.Where(a => a.AdmissionId != excludeAdmissionId.Value);
        return q.AnyAsync(ct);
    }

    public async Task<(int capacity, int occupied)> GetRoomOccupancyAsync(long wardRoomId, CancellationToken ct)
    {
        var room = await _db.Set<myWardRoom>().FirstOrDefaultAsync(r => r.WardRoomId == wardRoomId, ct);
        if (room is null) return (0, 0);

        var occupied = await _db.Set<myAdmission>()
            .CountAsync(a => !a.IsDeleted
                          && a.WardRoomId == wardRoomId
                          && a.Status == AdmissionStatus.Admitted
                          && a.DischargedAtUtc == null, ct);

        return (room.Capacity, occupied);
    }

    public Task<bool> WardRoomExistsAsync(long wardRoomId, CancellationToken ct)
        => _db.Set<myWardRoom>().AnyAsync(r => r.WardRoomId == wardRoomId, ct);

    public Task<bool> PatientExistsAsync(long patientId, CancellationToken ct)
        => _db.Set<HMS.Api.Features.Patient.Models.Entities.myPatient>()
              .AnyAsync(p => !p.IsDeleted && p.PatientId == patientId, ct);
}
