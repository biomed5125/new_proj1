using HMS.Module.Admission.Features.Admission.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace HMS.Module.Admission.Infrastructure.Persistence;

public sealed class AdmissionDbContext : DbContext
{
    public AdmissionDbContext(DbContextOptions<AdmissionDbContext> options) : base(options) { }

    public DbSet<myAdmission> Admissions => Set<myAdmission>();
    public DbSet<myWard> Wards => Set<myWard>();
    public DbSet<myRoomType> RoomTypes => Set<myRoomType>();
    public DbSet<myWardRoom> WardRooms => Set<myWardRoom>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // soft-delete filters
        modelBuilder.Entity<myAdmission>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<myWard>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<myRoomType>().HasQueryFilter(x => !x.IsDeleted);
        modelBuilder.Entity<myWardRoom>().HasQueryFilter(x => !x.IsDeleted);

        //// Module-scoped sequenc
        modelBuilder.HasSequence<long>("HmsIdSeq_Admission")
                    .StartsAt(2025400000000L)
                    .IncrementsBy(1);

        modelBuilder.UseHiLo("HmsIdSeq_Admission"); // use this for all long keys in this context

        base.OnModelCreating(modelBuilder);
    }
}
