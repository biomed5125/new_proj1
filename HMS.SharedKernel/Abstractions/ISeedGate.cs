using Microsoft.EntityFrameworkCore;

namespace HMS.SharedKernel.Abstractions
{
    public interface ISeedGate
    {
        Task<bool> ShouldRunAsync(DbContext db, string name, string version, CancellationToken ct);
        Task MarkRanAsync(DbContext db, string name, string version, CancellationToken ct);
    }
}
