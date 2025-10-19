//using HMS.Communication.Domain.Entities;
//using HMS.Communication.Infrastructure.Persistence;
//using HMS.Communication.Persistence;
//using Microsoft.EntityFrameworkCore;

//namespace HMS.Communication.Repositories;

//public sealed class RouterRuleRepository : IRouterRuleRepository
//{
//    private readonly CommunicationDbContext _db;
//    public RouterRuleRepository(CommunicationDbContext db) => _db = db;

//    public Task<List<RouterRule>> GetEnabledOrderedAsync(CancellationToken ct) =>
//        _db.RouterRules.AsNoTracking()
//            .Where(r => r.IsEnabled)
//            .OrderBy(r => r.Priority)
//            .ToListAsync(ct);

//    public async Task AddAsync(RouterRule rule, CancellationToken ct)
//    {
//        _db.RouterRules.Add(rule);
//        await _db.SaveChangesAsync(ct);
//    }

//    public async Task DisableAsync(long id, CancellationToken ct)
//    {
//        var r = await _db.RouterRules.SingleOrDefaultAsync(x => x.Id == id, ct);
//        if (r is null) return;
//        r.GetType().GetProperty(nameof(RouterRule.IsEnabled))!.SetValue(r, false);
//        await _db.SaveChangesAsync(ct);
//    }
//}
