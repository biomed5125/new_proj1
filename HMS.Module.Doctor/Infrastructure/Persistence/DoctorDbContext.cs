// Infrastructure/Persistence/DoctorDbContext.cs
using HMS.Module.Doctor.Features.Doctor.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HMS.Module.Doctor.Infrastructure.Persistence;

public sealed class DoctorDbContext : DbContext
{
    public DoctorDbContext(DbContextOptions<DoctorDbContext> options) : base(options) { }

    public DbSet<myDoctor> Doctors => Set<myDoctor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        modelBuilder.Entity<myDoctor>().HasQueryFilter(d => !d.IsDeleted);

        // Global HiLo sequence (per-DB)
        modelBuilder.HasSequence<long>("HmsIdSeq")
            .StartsAt(2025200000000L)
            .IncrementsBy(1);
        modelBuilder.UseHiLo("HmsIdSeq");

        base.OnModelCreating(modelBuilder);
    }
}
