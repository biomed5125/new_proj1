//using HMS.Module.Lab.Features.Lab.Infrastructure;
using HMS.Module.Lab.Features.Lab.Models.Entities;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Repositories;
public sealed class LabReadRepo : ILabReadRepo
{
    private readonly LabDbContext _db;
    public LabReadRepo(LabDbContext db) => _db = db;

    public Task<myLabRequest?> GetRequest(long id, CancellationToken ct)
        => _db.LabRequests.Include(x => x.Items).FirstOrDefaultAsync(x => x.LabRequestId == id, ct);

    public IQueryable<myLabTest> QueryTests() => _db.LabTests.AsNoTracking();
    public IQueryable<myLabPanel> QueryPanels() => _db.LabPanels.Include(p => p.Items).AsNoTracking();
}
