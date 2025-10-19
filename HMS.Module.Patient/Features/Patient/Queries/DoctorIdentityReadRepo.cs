

//using HMS.Module.Patient.Features.Patient.Repositories;
//using HMS.Module.Patient.Infrastructure.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Module.Patient.Features.Patient.Queries;

//public class DoctorIdentityReadRepo : IPatientReadRepo
//{
//    private readonly PatientDbContext _db;
//    public DoctorIdentityReadRepo(PatientDbContext db) => _db = db;

//    // (Id, Mrn, FullName) — MRN not applicable for doctors → return ""
//    public async Task<(long Id, string Mrn, string FullName)> GetDoctorIdentityAsync(long doctorId, CancellationToken ct)
//    {
//        var d = await _db.Set<myDoctor>().AsNoTracking()
//            .Where(x => x.DoctorId == doctorId && !x.IsDeleted)
//            .Select(x => new { x.DoctorId, FullName = x.FirstName + " " + x.LastName })
//            .FirstOrDefaultAsync(ct);

//        if (d is null) throw new InvalidOperationException("Doctor not found.");
//        return (d.DoctorId, "", d.FullName);
//    }
//}
