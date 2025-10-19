// HMS.Communication/Domain/Repositories/Interfaces/ICommDeviceRepository.cs
using HMS.Communication.Domain.Entities;
using HMS.Communication.Infrastructure.Persistence.Entities;
using System.Threading;
using System.Threading.Tasks;

namespace HMS.Communication.Domain.Repositories;

public interface ICommDeviceRepository
{
    Task<CommDevice?> GetByCodeAsync(string devicecode, CancellationToken ct);
    Task<CommDevice?> GetByIdAsync(long commdeviceid, CancellationToken ct);

    Task<long> EnsureAsync(string devicecode, string name, CancellationToken ct);
    Task<long> EnsureAsync(string devicecode, string name, string portName, long analyzerProfileId, CancellationToken ct);

    Task UpdatePortAsync(long commdeviceid, string portName, CancellationToken ct);
    Task UpdateProfileAsync(long commdeviceid, long analyzerProfileId, CancellationToken ct);
}

