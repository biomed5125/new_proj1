using HMS.Communication.Infrastructure.Persistence.Seed;
using HMS.Communication.Persistence;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Infrastructure.Persistence.Seed;
using HMS.SharedKernel.Abstractions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Endpoints;

public static class AdminMaintenanceEndpoints
{
    public static RouteGroupBuilder MapAdmin(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/admin")
            .AddEndpointFilter(new AdminKeyFilter());

        // status
        group.MapGet("/seed/status", async (LabDbContext lab, CommunicationDbContext comm) =>
        {
            var labRuns = await lab.SeedRuns.OrderByDescending(x => x.AppliedAtUtc).ToListAsync();
            var commRuns = await comm.SeedRuns.OrderByDescending(x => x.AppliedAtUtc).ToListAsync();
            return Results.Ok(new { lab = labRuns, comm = commRuns });
        });

        // apply demo seed now
        group.MapPost("/seed/run", async (IServiceProvider sp, IConfiguration cfg, ISeedGate gate) =>
        {
            var lab = sp.GetRequiredService<LabDbContext>();
            var comm = sp.GetRequiredService<CommunicationDbContext>();
            await lab.Database.MigrateAsync();
            await comm.Database.MigrateAsync();

            if (await gate.ShouldRunAsync(lab, "Lab:Demo:All", "v1", default))
            {
                await LabDbDemoSeed.EnsureAllAsync(lab, cfg);
                await gate.MarkRanAsync(lab, "Lab:Demo:All", "v1", default);
            }
            if (await gate.ShouldRunAsync(comm, "Comm:Devices:Demo", "v1", default))
            {
                await CommDeviceSeed.EnsureAsync(comm);
                await gate.MarkRanAsync(comm, "Comm:Devices:Demo", "v1", default);
            }
            return Results.Ok(new { ok = true });
        });

        app.MapHealthChecks("/health/details", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new {
                        key = e.Key,
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration.ToString()
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });

        // clear demo data (safe-ish): anything createdBy starts with "seed" or IsSeedData
        group.MapDelete("/seed/clear-demo", async (IConfiguration cfg, LabDbContext lab, CommunicationDbContext comm) =>
        {
            if (!cfg.GetValue<bool>("Seeding:AllowClearDemo"))
                return Results.BadRequest("Clearing demo is disabled.");

            // LAB demo wipe (example policy)
            await lab.Database.ExecuteSqlRawAsync("""
                DELETE FROM InstrumentTestMaps WHERE CreatedBy LIKE 'seed%';
                DELETE FROM LabPanelItems WHERE CreatedBy LIKE 'seed%';
                DELETE FROM LabPanels WHERE CreatedBy LIKE 'seed%';
                DELETE FROM LabResults WHERE CreatedBy LIKE 'seed%';
                DELETE FROM LabSamples WHERE CreatedBy LIKE 'seed%';
                DELETE FROM LabRequestItems WHERE CreatedBy LIKE 'seed%';
                DELETE FROM LabRequests WHERE CreatedBy LIKE 'seed%';
                DELETE FROM ReferenceRanges WHERE CreatedBy LIKE 'seed%';
                DELETE FROM LabTests WHERE CreatedBy LIKE 'seed%';
                DELETE FROM SpecimenTypes WHERE CreatedBy LIKE 'seed%';
            """);
            await lab.SaveChangesAsync();

            // COMM demo wipe
            await comm.Database.ExecuteSqlRawAsync("""
                DELETE FROM CommDevices WHERE Name LIKE '%Demo%' OR DeviceCode='ROCHE1';
            """);
            await comm.SaveChangesAsync();

            // clear seed ledger (so we can run again)
            lab.SeedRuns.RemoveRange(lab.SeedRuns);
            comm.SeedRuns.RemoveRange(comm.SeedRuns);
            await lab.SaveChangesAsync();
            await comm.SaveChangesAsync();

            return Results.Ok(new { ok = true, message = "Demo data cleared and seed ledger reset." });
        });

        // migrations
        group.MapPost("/db/migrate", async (LabDbContext lab, CommunicationDbContext comm) =>
        {
            await lab.Database.MigrateAsync();
            await comm.Database.MigrateAsync();
            return Results.Ok(new { ok = true });
        });

        // migrations status
        group.MapGet("/db/migrations", async (LabDbContext lab, CommunicationDbContext comm) =>
        {
            var labApplied = await lab.Database.GetAppliedMigrationsAsync();
            var commApplied = await comm.Database.GetAppliedMigrationsAsync();
            return Results.Ok(new { lab = labApplied, comm = commApplied });
        });

        // log tail (Serilog file)
        group.MapGet("/logs/tail", (IConfiguration cfg, int lines = 200) =>
        {
            var path = cfg["Logging:Serilog:Path"] ?? "logs/hms-.log";
            // today’s file (Serilog rolling)
            var today = path.Replace(".log", $"{DateTime.UtcNow:yyyyMMdd}.log");
            var file = File.Exists(today) ? today : path;
            if (!File.Exists(file)) return Results.NotFound("Log file not found.");
            var tail = TailFile(file, lines);
            return Results.Text(tail, "text/plain");
        });

        // change log level at runtime (Serilog)
        group.MapPost("/logs/level/{level}", (string level) =>
        {
            if (!Enum.TryParse<LogLevel>(level, true, out var _))
                return Results.BadRequest("Invalid level.");
            // If you use Serilog’s LoggingLevelSwitch, set it here.
            // For plain Microsoft logger, you’d typically rely on reloadable appsettings.
            return Results.Ok(new { ok = true, note = "If using Serilog LevelSwitch, wire it here." });
        });

        // info
        group.MapGet("/info", (IConfiguration cfg) =>
        {
            return Results.Ok(new
            {
                Version = typeof(AdminMaintenanceEndpoints).Assembly.GetName().Version?.ToString(),
                Seeding = new
                {
                    Enabled = cfg.GetValue<bool>("Seeding:Enabled"),
                    Mode = cfg["Seeding:Mode"],
                    AllowClearDemo = cfg.GetValue<bool>("Seeding:AllowClearDemo")
                },
                Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
            });
        });

        return group;
    }

    private static string TailFile(string path, int lines)
    {
        var q = new Queue<string>(lines);
        using var sr = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
        string? line;
        while ((line = sr.ReadLine()) != null)
        {
            if (q.Count == lines) q.Dequeue();
            q.Enqueue(line);
        }
        return string.Join(Environment.NewLine, q);
    }
}

/// <summary> Very simple API-key gate via header: X-Admin-Key </summary>
file sealed class AdminKeyFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext ctx, EndpointFilterDelegate next)
    {
        var cfg = ctx.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
        var expected = cfg["Admin:MaintenanceKey"] ?? "";
        var provided = ctx.HttpContext.Request.Headers["X-Admin-Key"].FirstOrDefault() ?? "";
        if (string.IsNullOrWhiteSpace(expected) || provided != expected)
            return Results.Unauthorized();
        return await next(ctx);
    }
}
