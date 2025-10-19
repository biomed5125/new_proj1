// HMS.Communication/CommunicationModuleRegistration.cs
using HMS.Communication.Abstractions;
using HMS.Communication.Application.Mapping;
using HMS.Communication.Application.Normalization;
using HMS.Communication.Application.Protocols.ASTM;
using HMS.Communication.Infrastructure.Drivers;
using HMS.Communication.Infrastructure.Drivers.Analyzers;
using HMS.Communication.Infrastructure.Observability;
using HMS.Communication.Infrastructure.Repositories;
using HMS.Communication.Integration;
using HMS.Communication.Integration.Lab;
using HMS.Communication.Persistence;
using HMS.Communication.Pipeline;
using HMS.Communication.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Communication;

public static class CommunicationModuleRegistration
{
    /// <summary>
    /// Registers the Communication module. 
    /// - Uses a DbContextFactory (safe to use from background workers).
    /// - Registers pipeline services and drivers.
    /// - Registers a Composite tracer (sinks added in API/Host).
    /// </summary>
    public static IServiceCollection AddCommunicationModule(
        this IServiceCollection services,
        string connectionString)
    {
        // IMPORTANT: Factory, not AddDbContext (so Host can use it from singletons/workers)
        services.AddDbContextFactory<CommunicationDbContext>(o =>
            o.UseSqlServer(connectionString,
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Comm")));

        // Composite tracer (fan-out). Concrete sinks are added in API/Host.
        services.AddSingleton<IFrameTracer, CompositeFrameTracer>();

        // Core pipeline
        services.AddScoped<IProtocolAdapter, AstmAdapter>();
        services.AddScoped<IRecordNormalizer, AstmRecordNormalizer>();
        services.AddScoped<IMessageRouter, DefaultMessageRouter>();
        services.AddScoped<IEventSink, EventSink>();
        services.AddScoped<IInstrumentToLisMapper, RocheCobasAstmMapper>();
        services.AddScoped<CommRouter>();

        // Lab integration
        services.AddScoped<ILabIntegrationService, LabIntegrationService>();

        // Bench helpers + drivers
        services.AddScoped<CommBenchService>();
        services.AddScoped<RocheCobasDriver>();
        services.AddScoped<SysmexSuitDriver>();
        services.AddScoped<FujiDryChemDriver>();
        services.AddScoped<IAnalyzerDriverFactory, AnalyzerDriverFactory>();
        services.AddScoped<IAnalyzerDriverResolver, AnalyzerDriverResolver>();

        // NO hosted services here. Host process owns background workers.
        return services;
    }
}
