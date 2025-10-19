// HMS.Module.Lab/Infrastructure/Persistence/Seed/LabDbSeedOrchestrator.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HMS.Module.Lab.Infrastructure.Persistence.Seed;

public static class LabDbSeedOrchestrator
{
    public static void SeedLab(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LabDbContext>();
        var cfg = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        db.Database.Migrate();
        LabDbDemoSeed.EnsureAllAsync(db, cfg).GetAwaiter().GetResult();
    }
}
