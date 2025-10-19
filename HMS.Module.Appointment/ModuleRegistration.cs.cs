using HMS.Module.Appointment.Features.Appointment.Repositories;
using HMS.Module.Appointment.Features.Appointment.Services;
using HMS.Module.Appointment.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Module.Appointment;

public static class ModuleRegistration
{
    public static IServiceCollection AddAppointmentModule(this IServiceCollection services, IConfiguration cfg,
        string csKey = "HmsDb_Appointment")
    {
        var cs = cfg.GetConnectionString(csKey)!;
        services.AddDbContext<AppointmentDbContext>(o =>
            o.UseSqlServer(cs, sql => sql.MigrationsAssembly(typeof(AppointmentDbContext).Assembly.FullName)));

        services.AddScoped<IAppointmentReadRepo, AppointmentReadRepo>();
        services.AddScoped<IAppointmentWriteRepo, AppointmentWriteRepo>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        return services;
    }
}