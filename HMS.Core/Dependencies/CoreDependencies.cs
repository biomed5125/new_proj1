using HMS.Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Core.DependencyInjection;

public static class Registration
{
    public static IServiceCollection AddCoreDb(this IServiceCollection services, IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString("HmsDb_Core")
                  ?? throw new InvalidOperationException("Missing connection string: HmsDb_Core");

        services.AddDbContext<CoreDbContext>(o =>
            o.UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "core"))); // IMPORTANT

        return services;
    }
}
