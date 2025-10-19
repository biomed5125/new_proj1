// LabModuleRegistration.cs
using FluentValidation;
using HMS.Module.Lab.Features.Lab.Dashboard;
using HMS.Module.Lab.Features.Lab.Endpoints;
using HMS.Module.Lab.Features.Lab.Endpoints.Patinets;
using HMS.Module.Lab.Features.Lab.Endpoints.Reports;
using HMS.Module.Lab.Features.Lab.Endpoints.Requests;
using HMS.Module.Lab.Features.Lab.Models.Dtos;
using HMS.Module.Lab.Features.Lab.Service;
using HMS.Module.Lab.Features.Lab.Validations;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Service;
using HMS.Module.Lab.Validations;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class LabModuleRegistration
{
    public static IServiceCollection AddLabModule(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<LabDbContext>(opt =>
            opt.UseSqlServer(cfg.GetConnectionString("HmsDb_Lab"),
                sql => sql.MigrationsHistoryTable("__EFMigrationsHistory", "Lab")));

        // Services
        services.AddScoped<ILabCatalogService, LabCatalogService>();
        services.AddScoped<ILabOrderService, LabOrderService>();
        services.AddScoped<ILabResultService, LabResultService>();
        services.AddScoped<ILabSampleService, LabSampleService>();
        services.AddScoped<ILabResultWriter,LabResultWriter>();

        // Dashboard
        services.AddScoped<ILabDashboardService, LabDashboardService>();

        // Validators
        services.AddScoped<IValidator<UpsertTestDto>, UpsertTestValidator>();
        services.AddScoped<IValidator<UpsertPanelDto>, UpsertPanelValidator>();
        services.AddScoped<IValidator<CreateLabRequestDto>, CreateLabRequestValidator>();
        services.AddScoped<IValidator<EnterResultDto>, EnterResultValidator>();
        services.AddScoped<IValidator<ApproveResultsDto>, ApproveResultsValidator>();

        return services;
    }

    public static IEndpointRouteBuilder MapLabModuleEndpoints(this IEndpointRouteBuilder app)
    {
        CatalogEndpoints.MapLabCatalogEndpoints(app);
        InstrumentMapEndpoints.MapInstrumentMapQueryEndpoints(app);
        InstrumentMapEndpoints.MapInstrumentMapMutationEndpoints(app);
        // Requests / Results / Samples
        RequestEndpoints.MapLabRequestEndpoints(app);
        ResultEndpoints.MapLabResultEndpoints(app);
        SampleEndpoints.MapLabSampleEndpoints(app);

        // Dashboard (NEW)
        LabDashboardEndpoints.MapLabDashboardEndpoints(app);

        LisMasterEndpoints.MapLisMasters(app);
        LisManualOrderEndpoints.MapLisManualOrders(app);
        LabelEndpoints.MapLabelEndpoints(app);
        //WalkInOrderEndpoints.MapWalkInOrderEndpoints(app);
        RefLookupsEndpoints.MapRefLookups(app);

        ReportEndpoints.MapReportEndpoints(app);
        LabHistoryEndpoints.MapLabHistoryEndpoints(app);
        EditRequestEndpoints.MapLabEditRequestEndpoints(app);
        LabPreanalyticalEndpoints.MapLabPreanalytical(app);
        LabPatientProfileEndpoints.MapLabPatientProfileEndpoints(app);
        app.MapLabOrders();

        return app;
    }
}
