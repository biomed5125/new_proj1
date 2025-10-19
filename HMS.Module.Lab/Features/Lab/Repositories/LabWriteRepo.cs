//using HMS.Module.Lab.Features.Lab.Infrastructure;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;

namespace HMS.Module.Lab.Repositories;
public sealed class LabWriteRepo : ILabWriteRepo
{
    private readonly LabDbContext _db;
    public LabWriteRepo(LabDbContext db) => _db = db;

    public Task AddRequest(myLabRequest req, CancellationToken ct) { _db.LabRequests.Add(req); return Task.CompletedTask; }
    public Task AddSample(myLabSample s, CancellationToken ct) { _db.LabSamples.Add(s); return Task.CompletedTask; }
    public Task Save(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
