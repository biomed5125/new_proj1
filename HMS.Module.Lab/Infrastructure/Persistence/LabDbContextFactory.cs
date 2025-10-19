using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HMS.Module.Lab.Infrastructure.Persistence;
public sealed class LabDbContextFactory : IDesignTimeDbContextFactory<LabDbContext>
{
    public LabDbContext CreateDbContext(string[] args)
    {
        var cfg = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json", optional: false)
        .AddEnvironmentVariables()
        .Build();

        var cs = cfg.GetConnectionString("HmsDb_Lab");
        var opt = new DbContextOptionsBuilder<LabDbContext>()
            .UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Lab"))
            .Options;

        return new LabDbContext(opt);
    }
}

//Add-Migration new_Lab -Project HMS.Module.Lab -StartupProject HMS.Api -Context LabDbContext -OutputDir Infrastructure/Persistence/Migrations
//"Server=DESKTOP-GM6JKUO\\SQLEXPRESS;Database=HMS_Lab;Trusted_Connection=True;TrustServerCertificate=True;";