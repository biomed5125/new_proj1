using HMS.Communication.Domain.Entities;
using HMS.Communication.Infrastructure.Persistence.Entities;
using HMS.SharedKernel.Base;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HMS.Communication.Persistence;
public sealed class CommunicationDbContext : DbContext
{
    public CommunicationDbContext(DbContextOptions<CommunicationDbContext> options) : base(options) { }

    public DbSet<CommEvent> Events => Set<CommEvent>();
    public DbSet<CommMessageInbound> Inbound => Set<CommMessageInbound>();
    public DbSet<CommMessageOutbound> Outbound => Set<CommMessageOutbound>();
    public DbSet<DeadLetterResult> DeadLetters => Set<DeadLetterResult>();
    public DbSet<CommDevice> CommDevices => Set<CommDevice>();
    public DbSet<AnalyzerProfile> AnalyzerProfiles => Set<AnalyzerProfile>();
    public DbSet<SeedRun> SeedRuns => Set<SeedRun>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(b);
    }
}
