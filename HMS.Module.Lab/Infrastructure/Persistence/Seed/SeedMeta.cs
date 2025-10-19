using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Lab.Infrastructure.Persistence.Seed
{
    public sealed class AppKv
    {
        public string Key { get; set; } = default!;
        public string? Value { get; set; }
    }

    public static class SeedMeta
    {
        public static async Task EnsureAsync(LabDbContext db)
        {
            if (!db.Model.GetEntityTypes().Any(t => t.ClrType == typeof(AppKv)))
                return; // add to your DbContext first (DbSet<AppKv> AppKv {get;set;})

            if (!await db.Set<AppKv>().AnyAsync())
                db.Set<AppKv>().Add(new AppKv { Key = "_init", Value = DateTime.UtcNow.ToString("O") });
            await db.SaveChangesAsync();
        }

        public static async Task<int> GetAsync(LabDbContext db, string key)
            => int.TryParse((await db.Set<AppKv>().FindAsync(key))?.Value, out var v) ? v : 0;

        public static async Task SetAsync(LabDbContext db, string key, int value)
        {
            var row = await db.Set<AppKv>().FindAsync(key);
            if (row == null) db.Set<AppKv>().Add(new AppKv { Key = key, Value = value.ToString() });
            else row.Value = value.ToString();
            await db.SaveChangesAsync();
        }
    }

}
