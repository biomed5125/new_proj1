using HMS.Api.Endpoints;
using HMS.Api.Endpoints.Barcodes;
using HMS.Api.Extensions;
using HMS.Api.Features.Communication.Bench;
using HMS.Api.Hubs;
using HMS.Api.Infrastructure.Seeding;
using HMS.Api.Observability;
using HMS.Api.Realtime;
using HMS.Communication;
using HMS.Communication.Abstractions;
using HMS.Communication.Infrastructure.Observability;
using HMS.Communication.Infrastructure.Persistence.Entities;
using HMS.Communication.Infrastructure.Persistence.Seed;
using HMS.Communication.Infrastructure.Repositories;
using HMS.Communication.Persistence;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Module.Lab.Infrastructure.Persistence.Seed;
using HMS.Realtime.Abstractions;
using HMS.SharedKernel.Abstractions;
using HMS.SharedKernel.Ids;
using HMS.SharedServices.IdGeneration;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;
var logPath = builder.Configuration["Logging:Serilog:Path"] ?? "logs/hms-.log";
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        path: logPath,
        rollingInterval: RollingInterval.Day,
        restrictedToMinimumLevel: LogEventLevel.Information,
        retainedFileCountLimit: 7)
    .CreateLogger();
builder.Host.UseSerilog();

// Business IDs (singleton is fine for API)
builder.Services.AddSingleton<IBusinessIdGenerator, TimeBasedBusinessIdGenerator>();

var seedingEnabled = cfg.GetValue<bool>("Seeding:Enabled");
var seedMode = cfg.GetValue<string>("Seeding:Mode"); // Off|Demo|ProdMinimal

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => c.CustomSchemaIds(t => t.FullName!.Replace("+", ".")));

// ---- Tracing in API process: SignalR only (no DB/File here) ----
builder.Services.AddSignalR();
builder.Services.AddSingleton<ILabRealtime, SignalRLabRealtime>(); // real broadcaster
builder.Services.AddSingleton<ITraceBroadcaster, TraceBroadcaster>();
builder.Services.AddSingleton<IFrameTracer, SignalRFrameTracer>();
builder.Services.AddSingleton<IFrameTracer, CompositeFrameTracer>(); // fans-out to SignalRFrameTracer
// Individual tracers (register them as IFrameTracer)
builder.Services.AddSingleton<IFrameTracer>(sp =>new FileFrameTracer(Path.Combine(@"D:\HMS\comm\trace", "trace.log")));

// Razor pages(Lab UI)
builder.Services.AddLabRazorRoutes();   // ⬅️ this replaces the long AddRazorPages block

// Razor + JSON enums
builder.Services.ConfigureHttpJsonOptions(o =>
{
    o.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<ISeedGate, SeedGate>();

// ---------------- Lab module ----------------
builder.Services.AddLabModule(cfg); // your existing Lab module reg.
builder.Services.AddDbContext<LabDbContext>(o =>
{
    o.EnableDetailedErrors();
    o.EnableSensitiveDataLogging();
});

// ---------------- Communication module ----------------
// Registers Comm DbContext (scoped) and all pipeline services (scoped).
builder.Services.AddCommunicationModule(cfg.GetConnectionString("HmsDb_Comm")! );
builder.Services.AddScoped<CommBenchService>();

builder.Services.AddCors(o =>
{
    o.AddPolicy("ui", p => p
        .WithOrigins(
            "https://localhost:7190", // same-host (safe)
            "http://localhost:7190",
            "https://localhost:5173", // adjust if your UI dev server differs
            "http://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<LabDbContext>("lab-db")
    .AddDbContextCheck<CommunicationDbContext>("comm-db");


var app = builder.Build();

app.UseCors("ui");

// ---------------------------------------------------------------------------------
// Middleware
// ---------------------------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();



// ---------------------------------------------------------------------------------
// Migrate + Seed (only here!)
// ---------------------------------------------------------------------------------
// --- Seed Lab + Communication databases (idempotent) ---
using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    var gate = sp.GetRequiredService<ISeedGate>();

    // LAB
    var lab = sp.GetRequiredService<LabDbContext>();
    await lab.Database.MigrateAsync();

    if (seedingEnabled && !string.Equals(seedMode, "Off", StringComparison.OrdinalIgnoreCase))
    {
        // example: run demo seed once per version
        if (await gate.ShouldRunAsync(lab, "Lab:Demo:All", "v1", CancellationToken.None))
        {
            await LabDbDemoSeed.EnsureAllAsync(lab, cfg, CancellationToken.None);
            await gate.MarkRanAsync(lab, "Lab:Demo:All", "v1", CancellationToken.None);
        }
    }

    // COMM
    var comm = sp.GetRequiredService<CommunicationDbContext>();

    await comm.Database.MigrateAsync();

    if (seedingEnabled && !string.Equals(seedMode, "Off", StringComparison.OrdinalIgnoreCase))
    {
        if (await gate.ShouldRunAsync(comm, "Comm:Devices:Demo", "v1", CancellationToken.None))
        {
            await CommDeviceSeed.EnsureAsync(comm, CancellationToken.None);
            await gate.MarkRanAsync(comm, "Comm:Devices:Demo", "v1", CancellationToken.None);
        }
    }
}


app.MapAdmin();
app.MapHealthChecks("/health");
// ---------------------------------------------------------------------------------
// Endpoints / Hubs / Pages
// ---------------------------------------------------------------------------------
app.MapLabModuleEndpoints();
app.MapBarcodeEndpoints();
app.MapBarcodeImageEndpoints();
app.MapBarcodeScanEndpoint();

// CommBench REST helpers (lists, push, send-simple, etc.)
app.MapCommBenchLists();
app.MapCommBenchPush();

// SignalR hubs
// Map hub
app.MapHub<CommTraceHub>("/hubs/commtrace"); // raw in/out frames
app.MapHub<LabHub>("/hubs/lab"); // normalized “ResultPosted”

// Razor Pages
app.MapControllers();
app.MapRazorPages();

// Root redirect
if (app.Environment.IsDevelopment())
    app.MapGet("/", () => Results.Redirect("/lab"));
else
    app.MapGet("/", () => Results.Redirect("/swagger"));

await app.RunAsync();
