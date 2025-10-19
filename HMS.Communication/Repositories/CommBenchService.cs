using HMS.Communication.Abstractions;
using HMS.Communication.Domain.Entities;
using HMS.Communication.Infrastructure.Drivers;
using HMS.Communication.Infrastructure.Drivers.Analyzers;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Communication.Infrastructure.Repositories
{
    public sealed class CommBenchService
    {
        private readonly CommunicationDbContext _db;
        private readonly IAnalyzerDriverResolver _driverResolver;

        public CommBenchService(CommunicationDbContext db, IAnalyzerDriverResolver resolver)
        {
            _db = db;
            _driverResolver = resolver;
        }

        // HMS.Communication.Infrastructure.Repositories/CommBenchService.cs
        public async Task<byte[]> BuildTestFrameAsync(long deviceId, string accession, string testCode, string value, string unit, CancellationToken ct)
        {
            var device = await _db.CommDevices
                .Include(d => d.AnalyzerProfile)
                .FirstOrDefaultAsync(d => d.CommDeviceId == deviceId, ct);

            if (device is null)
                throw new InvalidOperationException($"Device {deviceId} not found.");

            var (driver, deviceCode, mode) = _driverResolver.Resolve(device);
            return driver.BuildSimpleResult(accession, testCode, value, unit, deviceCode, mode);
        }

    }
}
