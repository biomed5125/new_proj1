using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Features.Admission.Models.Enums;
using HMS.Module.Admission.Features.Admission.Queries;
using HMS.Module.Admission.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Admission.Features.Admission.Repositories;

public sealed class AdmissionReadRepo : IAdmissionReadRepo
{
    private readonly AdmissionDbContext _db;
    public AdmissionReadRepo(AdmissionDbContext db) => _db = db;

    public Task<myAdmission?> GetAsync(long id, CancellationToken ct)
        => _db.Admissions.AsNoTracking().FirstOrDefaultAsync(x => x.AdmissionId == id, ct);

    public async Task<IReadOnlyList<myAdmission>> ListAsync(AdmissionQuery q, CancellationToken ct)
    {
        var baseQ = _db.Admissions.AsNoTracking().AsQueryable();

        if (q.PatientId is { } pid) baseQ = baseQ.Where(x => x.PatientId == pid);
        if (q.Status is { } st) baseQ = baseQ.Where(x => (int)x.Status == st);
        if (q.FromUtc is { } f) baseQ = baseQ.Where(x => x.AdmittedAtUtc >= f);
        if (q.ToUtc is { } t) baseQ = baseQ.Where(x => x.AdmittedAtUtc < t);

        return await baseQ
            .OrderByDescending(x => x.AdmittedAtUtc)
            .Take(500)
            .ToListAsync(ct);
    }

    public Task<bool> HasOpenAsync(long patientId, CancellationToken ct)
        => _db.Admissions.AsNoTracking()
            .AnyAsync(x => x.PatientId == patientId && x.Status == AdmissionStatus.Admitted && !x.IsDeleted, ct);
}
