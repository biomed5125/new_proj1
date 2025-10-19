using Microsoft.EntityFrameworkCore;
using System.Reflection;
using HMS.Module.Appointment.Features.Appointment.Models.Entities;

namespace HMS.Module.Appointment.Infrastructure.Persistence;

public class AppointmentDbContext : DbContext
{
    public AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : base(options) { }

    public DbSet<myAppointment> Appointments => Set<myAppointment>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        mb.Entity<myAppointment>().HasQueryFilter(a => !a.IsDeleted);

        mb.HasSequence<long>("HmsIdSeq").StartsAt(2025300000000L).IncrementsBy(1);
        mb.UseHiLo("HmsIdSeq");

        base.OnModelCreating(mb);
    }
}
