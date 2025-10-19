// HMS.Communication/Repositories/Implementation/CommDeviceRepository.cs
using HMS.Communication.Domain.Entities;
using HMS.Communication.Domain.Repositories;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Infrastructure.Persistence.Entities;
using HMS.Communication.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Communication.Repositories;

public sealed class CommDeviceRepository : ICommDeviceRepository
{
    private readonly CommunicationDbContext _db;
    public CommDeviceRepository(CommunicationDbContext db) => _db = db;

    public Task<CommDevice?> GetByCodeAsync(string code, CancellationToken ct)
        => _db.CommDevices.AsNoTracking()
              .Include(d => d.AnalyzerProfile)
              .FirstOrDefaultAsync(d => d.DeviceCode == code, ct);

    public Task<CommDevice?> GetByIdAsync(long id, CancellationToken ct)
        => _db.CommDevices.AsNoTracking()
              .Include(d => d.AnalyzerProfile)
              .FirstOrDefaultAsync(d => d.CommDeviceId == id, ct);

    public async Task<long> EnsureAsync(string code, string name, CancellationToken ct)
    {
        var existing = await _db.CommDevices.FirstOrDefaultAsync(d => d.DeviceCode == code, ct);
        if (existing is not null) return existing.CommDeviceId;

        var row = new CommDevice { DeviceCode = code, Name = name, IsActive = true, PortName = "" };
        _db.CommDevices.Add(row);
        await _db.SaveChangesAsync(ct);
        return row.CommDeviceId;
    }

    public async Task<long> EnsureAsync(string code, string name, string portName, long analyzerProfileId, CancellationToken ct)
    {
        var existing = await _db.CommDevices.FirstOrDefaultAsync(d => d.DeviceCode == code, ct);
        if (existing is not null) return existing.CommDeviceId;

        var row = new CommDevice
        {
            DeviceCode = code,
            Name = name,
            PortName = portName,
            AnalyzerProfileId = analyzerProfileId,
            IsActive = true
        };
        _db.CommDevices.Add(row);
        await _db.SaveChangesAsync(ct);
        return row.CommDeviceId;
    }

    public async Task UpdatePortAsync(long id, string portName, CancellationToken ct)
    {
        var row = await _db.CommDevices.FirstOrDefaultAsync(d => d.CommDeviceId == id, ct);
        if (row is null) return;
        row.PortName = portName;
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpdateProfileAsync(long id, long analyzerProfileId, CancellationToken ct)
    {
        var row = await _db.CommDevices.FirstOrDefaultAsync(d => d.CommDeviceId == id, ct);
        if (row is null) return;
        row.AnalyzerProfileId = analyzerProfileId;
        await _db.SaveChangesAsync(ct);
    }
}
