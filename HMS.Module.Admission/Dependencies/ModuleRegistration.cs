using HMS.Module.Admission.Features.Admission.Endpoints;
using HMS.Module.Admission.Features.Admission.Repositories;
using HMS.Module.Admission.Features.Admission.Services;
using HMS.Module.Admission.Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Module.Admission.Dependencies;

public static class ModuleRegistration
{
    public static IServiceCollection AddAdmissionModule(this IServiceCollection services, IConfiguration cfg)
    {
        var cs = cfg.GetConnectionString("HmsDb_Admission")
                 ?? throw new InvalidOperationException("Missing connection string: HmsDb_Admission");

        services.AddDbContext<AdmissionDbContext>(o =>
            o.UseSqlServer(cs, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "admission")));

        services.AddScoped<IAdmissionReadRepo, AdmissionReadRepo>();
        services.AddScoped<IAdmissionWriteRepo, AdmissionWriteRepo>();
        services.AddScoped<IAdmissionService, AdmissionService>();

        return services;
    }
}
