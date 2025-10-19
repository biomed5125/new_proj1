using HMS.Communication.Domain.Entities;

namespace HMS.Communication.Repositories;

public interface IRouterRuleRepository
{
    Task<List<RouterRule>> GetEnabledOrderedAsync(CancellationToken ct);
    Task AddAsync(RouterRule rule, CancellationToken ct);
    Task DisableAsync(long id, CancellationToken ct);
}
