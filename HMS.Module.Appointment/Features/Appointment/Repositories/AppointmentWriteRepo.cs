using HMS.Module.Appointment.Features.Appointment.Models.Entities;
using HMS.Module.Appointment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Module.Appointment.Features.Appointment.Repositories;

public sealed class AppointmentWriteRepo : IAppointmentWriteRepo
{
    private readonly AppointmentDbContext _db;
    public AppointmentWriteRepo(AppointmentDbContext db) => _db = db;

    public Task AddAsync(myAppointment a, CancellationToken ct) =>
        _db.AddAsync(a, ct).AsTask();

    public Task<myAppointment?> GetAsync(long id, CancellationToken ct) =>
        _db.Set<myAppointment>().FirstOrDefaultAsync(x => x.AppointmentId == id && !x.IsDeleted, ct);

    public Task SaveAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}
