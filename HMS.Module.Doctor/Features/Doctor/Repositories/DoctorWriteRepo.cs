// Features/Doctor/Repositories/DoctorWriteRepo.cs
using HMS.Module.Doctor.Features.Doctor.Models.Entities;
using HMS.Module.Doctor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Doctor.Features.Doctor.Repositories;

public sealed class DoctorWriteRepo : IDoctorWriteRepo
{
    private readonly DoctorDbContext _db;
    public DoctorWriteRepo(DoctorDbContext db) => _db = db;

    public Task AddAsync(myDoctor e, CancellationToken ct)
    { _db.Doctors.Add(e); return Task.CompletedTask; }

    public Task UpdateAsync(myDoctor e, CancellationToken ct)
    { _db.Doctors.Update(e); return Task.CompletedTask; }

    public async Task<bool> SoftDeleteAsync(long id, CancellationToken ct)
    {
        var e = await _db.Doctors.FirstOrDefaultAsync(x => x.DoctorId == id, ct);
        if (e is null) return false;
        e.IsDeleted = true; e.UpdatedAt = DateTime.UtcNow; e.UpdatedBy = "api";
        _db.Doctors.Update(e);
        return true;
    }

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
