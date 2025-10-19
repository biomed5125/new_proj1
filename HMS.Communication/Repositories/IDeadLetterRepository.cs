// HMS.Communication/Domain/Repositories/Interfaces/IDeadLetterRepository.cs
using System.Threading;
using System.Threading.Tasks;
using HMS.Communication.Domain.Entities;

namespace HMS.Communication.Domain.Repositories;

public interface IDeadLetterRepository
{
    Task<long> AddAsync(DeadLetterResult row, CancellationToken ct);
}
