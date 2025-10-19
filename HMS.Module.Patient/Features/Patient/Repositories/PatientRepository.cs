//using HMS.Api.Features.Patient.Models.Dtos;
//using HMS.Api.Features.Patient.Models.Entities;
//using HMS.Api.Features.Patient.Queries;
//using HMS.Api.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Api.Features.Patient.Repositories
//{

//    public class PatientRepository : IPatientRepository
//    {
//        private readonly HmsDbContext _db;
//        public PatientRepository(HmsDbContext db) => _db = db;

//        public async Task<PatientDto?> GetAsync(long id, CancellationToken ct)
//            => await _db.Set<myPatient>().AsNoTracking()
//                .Where(p => !p.IsDeleted && p.PatientId == id)
//                .Select(p => new PatientDto(
//                    p.PatientId, p.Mrn ?? "", p.FirstName ?? "", p.LastName ?? "",
//                    p.DateOfBirth, p.Gender, p.Phone, p.Email))
//                .FirstOrDefaultAsync(ct);

//        public async Task<IReadOnlyList<PatientDto>> ListAsync(PatientQuery q, CancellationToken ct)
//        {
//            var baseQ = _db.Set<myPatient>().AsNoTracking().Where(p => !p.IsDeleted);

//            if (!string.IsNullOrWhiteSpace(q.Search))
//            {
//                var s = q.Search.Trim();
//                baseQ = baseQ.Where(p =>
//                    (p.Mrn ?? "").Contains(s) ||
//                    (p.FirstName ?? "").Contains(s) ||
//                    (p.LastName ?? "").Contains(s));
//            }

//            return await baseQ
//                .OrderBy(p => p.LastName).ThenBy(p => p.FirstName)
//                .Skip((q.Page - 1) * q.PageSize)
//                .Take(q.PageSize)
//                .Select(p => new PatientDto(
//                    p.PatientId, p.Mrn ?? "", p.FirstName ?? "", p.LastName ?? "",
//                    p.DateOfBirth, p.Gender, p.Phone, p.Email))
//                .ToListAsync(ct);
//        }
//    }
//}