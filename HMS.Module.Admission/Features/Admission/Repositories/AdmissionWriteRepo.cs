using HMS.Module.Admission.Features.Admission.Models.Entities;
using HMS.Module.Admission.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Admission.Features.Admission.Repositories;

public sealed class AdmissionWriteRepo : IAdmissionWriteRepo
{
    private readonly AdmissionDbContext _db;
    public AdmissionWriteRepo(AdmissionDbContext db) => _db = db;

    public async Task AddAsync(myAdmission e, CancellationToken ct)
        => await _db.Admissions.AddAsync(e, ct);

    public Task UpdateAsync(myAdmission e, CancellationToken ct)
    {
        _db.Admissions.Update(e);
        return Task.CompletedTask;
    }

    public async Task SoftDeleteAsync(long id, CancellationToken ct)
    {
        var e = await _db.Admissions.FirstOrDefaultAsync(x => x.AdmissionId == id, ct);
        if (e is null) return;
        e.IsDeleted = true;
        _db.Admissions.Update(e);
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
