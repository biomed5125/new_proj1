using System;
using System.IO;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HMS.Communication.Infrastructure.Persistence
{
    // Used only by EF Core tools at design-time (Add-Migration / Update-Database).
    public sealed class CommunicationDbContextFactory
        : IDesignTimeDbContextFactory<CommunicationDbContext>
    {
        public CommunicationDbContext CreateDbContext(string[] args)
        {
            var cfg = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

            var cs = cfg.GetConnectionString("HmsDb_Comm");
            var opt = new DbContextOptionsBuilder<CommunicationDbContext>()
                .UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Comm"))
                .Options;

            return new CommunicationDbContext(opt);
        }
    }
}
