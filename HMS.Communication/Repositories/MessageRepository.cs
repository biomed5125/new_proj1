// HMS.Communication/Repositories/Implementation/MessageRepository.cs
using System.Threading;
using System.Threading.Tasks;
using HMS.Communication.Domain.Entities;
using HMS.Communication.Domain.Repositories;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Persistence;

namespace HMS.Communication.Repositories;

public sealed class MessageRepository : IMessageRepository
{
    private readonly CommunicationDbContext _db;
    public MessageRepository(CommunicationDbContext db) => _db = db;

    public async Task<long> AddInboundAsync(CommMessageInbound msg, CancellationToken ct)
    {
        _db.Inbound.Add(msg);
        await _db.SaveChangesAsync(ct);
        return msg.CommMessageInboundId;
    }

    public async Task<long> AddOutboundAsync(CommMessageOutbound msg, CancellationToken ct)
    {
        _db.Outbound.Add(msg);
        await _db.SaveChangesAsync(ct);
        return msg.CommMessageOutboundId;
    }

    public async Task MarkOutboundSentAsync(long id, CancellationToken ct)
    {
        var row = await _db.Outbound.FindAsync(new object[] { id }, ct);
        if (row is null) return;
        row.Sent = true;
        await _db.SaveChangesAsync(ct);
    }
}
