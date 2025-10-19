// HMS.Module.Doctor/Infrastructure/Persistence/DoctorDbContextFactory.cs
using HMS.Module.Doctor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

public sealed class DoctorDbContextFactory : IDesignTimeDbContextFactory<DoctorDbContext>
{
    public DoctorDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("HmsDb_Doctor")
                 ?? Environment.GetEnvironmentVariable("HMS_DB_DOCTOR")
                 ?? "Server=.;Database=HMS_Doctor;Trusted_Connection=True;TrustServerCertificate=True;";

        var opts = new DbContextOptionsBuilder<DoctorDbContext>()
            .UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "doctor"))
            .Options;

        return new DoctorDbContext(opts);
    }
}
