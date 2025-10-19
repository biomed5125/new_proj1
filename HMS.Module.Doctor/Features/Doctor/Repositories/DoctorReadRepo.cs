// Features/Doctor/Repositories/DoctorReadRepo.cs
using HMS.Module.Doctor.Features.Doctor.Models.Entities;
using HMS.Module.Doctor.Features.Doctor.Queries;
using HMS.Module.Doctor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Doctor.Features.Doctor.Repositories;

public sealed class DoctorReadRepo : IDoctorReadRepo
{
    private readonly DoctorDbContext _db;
    public DoctorReadRepo(DoctorDbContext db) => _db = db;

    public Task<myDoctor?> GetAsync(long id, CancellationToken ct)
        => _db.Doctors.AsNoTracking().FirstOrDefaultAsync(x => x.DoctorId == id, ct);

    public async Task<IReadOnlyList<myDoctor>> ListAsync(DoctorQuery q, CancellationToken ct)
    {
        var baseQ = _db.Doctors.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            baseQ = baseQ.Where(d =>
                d.FirstName.Contains(s) || d.LastName.Contains(s) ||
                d.LicenseNumber.Contains(s) || (d.Phone ?? "").Contains(s) || (d.Email ?? "").Contains(s));
        }

        if (q.IsActive is not null)
            baseQ = baseQ.Where(d => d.IsActive == q.IsActive);

        return await baseQ.OrderBy(d => d.LastName).ThenBy(d => d.FirstName).ToListAsync(ct);
    }

    public Task<bool> LicenseExistsAsync(string license, long? excludeId, CancellationToken ct)
        => _db.Doctors.AsNoTracking()
             .Where(d => d.LicenseNumber == license && !d.IsDeleted && (excludeId == null || d.DoctorId != excludeId))
             .AnyAsync(ct);
}
