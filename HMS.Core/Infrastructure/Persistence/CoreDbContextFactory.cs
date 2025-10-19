// HMS.Core/Infrastructure/Persistence/CoreDbContextFactory.cs
using HMS.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HMS.Core.Infrastructure.Persistence;

public sealed class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        // load appsettings to get HmsDb_Core at design-time
        var cfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = cfg.GetConnectionString("HmsDb_Core")
                 ?? "Server=DESKTOP-GM6JKUO\\SQLEXPRESS;Database=HMS_Core;Trusted_Connection=True;TrustServerCertificate=True;";

        var opts = new DbContextOptionsBuilder<CoreDbContext>()
            .UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "core")) // SAME as runtime
            .Options;

        return new CoreDbContext(opts);
    }
}
