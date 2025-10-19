using HMS.SharedKernel.Abstractions;
using HMS.SharedKernel.Base;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Infrastructure.Seeding;

/// <summary> Tracks which named seeds (with version) were applied. </summary>
//public interface ISeedGate
//{
//    Task<bool> ShouldRunAsync(DbContext db, string name, string version, CancellationToken ct);
//    Task MarkRanAsync(DbContext db, string name, string version, CancellationToken ct);
//}

public sealed class SeedGate : ISeedGate
{
    public async Task<bool> ShouldRunAsync(DbContext db, string name, string version, CancellationToken ct)
        => !await db.Set<SeedRun>().AnyAsync(x => x.Name == name && x.Version == version, ct);

    public async Task MarkRanAsync(DbContext db, string name, string version, CancellationToken ct)
    {
        db.Set<SeedRun>().Add(new SeedRun { Name = name, Version = version });
        await db.SaveChangesAsync(ct);
    }
}
