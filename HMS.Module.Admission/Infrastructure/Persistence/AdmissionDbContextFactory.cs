// HMS.Module.Admission/Infrastructure/Persistence/AdmissionDbContextFactory.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HMS.Module.Admission.Infrastructure.Persistence;

public sealed class AdmissionDbContextFactory : IDesignTimeDbContextFactory<AdmissionDbContext>
{
    public AdmissionDbContext CreateDbContext(string[] args)
    {
        var cfg = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

        var cs = cfg.GetConnectionString("HmsDb_Admission");
        var opt = new DbContextOptionsBuilder<AdmissionDbContext>()
            .UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "admission"))
            .Options;

        return new AdmissionDbContext(opt);
    }
}
