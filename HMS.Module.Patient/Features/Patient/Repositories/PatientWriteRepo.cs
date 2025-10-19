
using HMS.Module.Patient.Features.Patient.Models.Entities;
using HMS.Module.Patient.Features.Patient.Repositories;
using HMS.Module.Patient.Infrastructure.Persistence;
using HMS.SharedKernel.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace HMS.Module.Patient.Features.Patient.Repositories
{
    public class PatientWriteRepo : IPatientWriteRepo
    {
        private readonly PatientDbContext _db;
        public PatientWriteRepo(PatientDbContext db) => _db = db;

        public Task<bool> MrnExistsAsync(string mrn, CancellationToken ct)
            => _db.Set<myPatient>().AnyAsync(p => !p.IsDeleted && p.Mrn == mrn, ct);

        public async Task AddAsync(myPatient e, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            using var tx = await _db.Database.BeginTransactionAsync(ct);
            _db.Patients.Add(e);

            var payload = JsonSerializer.Serialize(new
            {
                e.PatientId,
                e.Mrn,
                e.FirstName,
                e.LastName,
                e.DateOfBirth,
                e.Phone
            });

            _db.Set<OutboxEvent>().Add(new OutboxEvent
            {
                Stream = "patient",
                Type = "Patient.Upserted",
                Payload = payload,
                OccurredAtUtc = now
            });

            await tx.CommitAsync(ct);
            await _db.AddAsync(e, ct);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(myPatient e, CancellationToken ct)
        {
            _db.Update(e);
            await _db.SaveChangesAsync(ct);
        }

        public async Task SoftDeleteAsync(long id, CancellationToken ct)
        {
            var e = await _db.Set<myPatient>().FirstOrDefaultAsync(p => p.PatientId == id && !p.IsDeleted, ct);
            if (e is null) return;
            e.IsDeleted = true;
            e.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
        
    }
}
