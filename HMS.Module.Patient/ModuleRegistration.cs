using HMS.Module.Patient.Features.Patient.Repositories;
using HMS.Module.Patient.Features.Patient.Services;
using HMS.Module.Patient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HMS.Module.Patient;

public static class ModuleRegistration
{
    public static IServiceCollection AddPatientModule(this IServiceCollection services, IConfiguration cfg,
        string csKey = "HmsDb_Patient")
    {
        var cs = cfg.GetConnectionString(csKey)!;
        services.AddDbContext<PatientDbContext>(o =>
            o.UseSqlServer(cs, sql => sql.MigrationsAssembly(typeof(PatientDbContext).Assembly.FullName)));

        // repos + services
        services.AddScoped<IPatientReadRepo, PatientReadRepo>();
        services.AddScoped<IPatientWriteRepo, PatientWriteRepo>();
        services.AddScoped<IPatientService, PatientService>();
        return services;
    }
}

//Add-Migration Init_Patient -Project HMS.Module.Patient -StartupProject HMS.Api `-Context PatientDbContext -OutputDir Infrastructure/Persistence/Migrations