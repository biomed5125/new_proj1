using HMS.Module.Patient.Features.Patient.Models.Dtos;
using HMS.Module.Patient.Features.Patient.Models.Entities;
using HMS.Module.Patient.Features.Patient.Queries;
using HMS.Module.Patient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Patient.Features.Patient.Repositories;

public class PatientReadRepo : IPatientReadRepo
{
    private readonly PatientDbContext _db;
    public PatientReadRepo(PatientDbContext db) => _db = db;

    public async Task<PatientDto?> GetAsync(long id, CancellationToken ct)
    {
        return await _db.Set<myPatient>()
            .AsNoTracking()
            .Where(p => p.PatientId == id)
            .Select(p => new PatientDto
            {
                PatientId = p.PatientId,
                Mrn = p.Mrn ?? "",
                FirstName = p.FirstName ?? "",
                LastName = p.LastName ?? "",
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Phone = p.Phone,
                Email = p.Email
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<PatientDto>> ListAsync(PatientQuery q, CancellationToken ct)
    {
        var baseQ = _db.Set<myPatient>().AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q.Search))
        {
            var s = q.Search.Trim();
            baseQ = baseQ.Where(p =>
                (p.Mrn != null && p.Mrn.Contains(s)) ||
                (p.FirstName != null && p.FirstName.Contains(s)) ||
                (p.LastName != null && p.LastName.Contains(s)) ||
                (p.Phone != null && p.Phone.Contains(s)));
        }

        return await baseQ
            .OrderBy(p => p.FirstName).ThenBy(p => p.LastName)
            .Select(p => new PatientDto
            {
                PatientId = p.PatientId,
                Mrn = p.Mrn ?? "",
                FirstName = p.FirstName ?? "",
                LastName = p.LastName ?? "",
                DateOfBirth = p.DateOfBirth,
                Gender = p.Gender,
                Phone = p.Phone,
                Email = p.Email
            })
            .ToListAsync(ct);
    }
}
