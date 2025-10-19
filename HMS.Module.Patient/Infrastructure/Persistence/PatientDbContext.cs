using HMS.Module.Patient.Features.Patient.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HMS.Module.Patient.Infrastructure.Persistence;

public class PatientDbContext : DbContext
{
    public PatientDbContext(DbContextOptions<PatientDbContext> options) : base(options) { }
    public DbSet<myPatient> Patients => Set<myPatient>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        mb.Entity<myPatient>().HasQueryFilter(p => !p.IsDeleted);

        mb.HasSequence<long>("HmsIdSeq").StartsAt(2025100000000L).IncrementsBy(1);
        mb.UseHiLo("HmsIdSeq");

        base.OnModelCreating(mb);
    }
}
