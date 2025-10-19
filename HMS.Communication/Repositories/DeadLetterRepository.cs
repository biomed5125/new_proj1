// HMS.Communication/Repositories/Implementation/DeadLetterRepository.cs
using System.Threading;
using System.Threading.Tasks;
using HMS.Communication.Domain.Entities;
using HMS.Communication.Domain.Repositories;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Persistence;

namespace HMS.Communication.Repositories;

public sealed class DeadLetterRepository : IDeadLetterRepository
{
    private readonly CommunicationDbContext _db;
    public DeadLetterRepository(CommunicationDbContext db) => _db = db;

    public async Task<long> AddAsync(DeadLetterResult row, CancellationToken ct)
    {
        _db.DeadLetters.Add(row);
        await _db.SaveChangesAsync(ct);
        return row.DeadLetterResultId;
    }
}
