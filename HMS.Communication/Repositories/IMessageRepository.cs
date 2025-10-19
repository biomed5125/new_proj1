// HMS.Communication/Domain/Repositories/Interfaces/IMessageRepository.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HMS.Communication.Domain.Entities;

namespace HMS.Communication.Domain.Repositories;

public interface IMessageRepository
{
    Task<long> AddInboundAsync(CommMessageInbound msg, CancellationToken ct);
    Task<long> AddOutboundAsync(CommMessageOutbound msg, CancellationToken ct);
    Task MarkOutboundSentAsync(long id, CancellationToken ct);
}
