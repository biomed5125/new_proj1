// Dependencies/ModuleRegistration.cs
using FluentValidation;
using HMS.Module.Doctor.Features.Doctor.Repositories;
using HMS.Module.Doctor.Features.Doctor.Services;
using HMS.Module.Doctor.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Module.Doctor.Dependencies;

public static class ModuleRegistration
{
    public static IServiceCollection AddDoctorModule(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("HmsDb_Doctor")
                 ?? throw new InvalidOperationException("Missing connection string: HmsDb_Doctor");

        services.AddDbContext<DoctorDbContext>(o =>
            o.UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "doctor")));

        services.AddScoped<IDoctorReadRepo, DoctorReadRepo>();
        services.AddScoped<IDoctorWriteRepo, DoctorWriteRepo>();
        services.AddScoped<IDoctorService, DoctorService>();

        services.AddValidatorsFromAssembly(typeof(ModuleRegistration).Assembly);
        return services;
    }
}
