using HMS.Api.Realtime;
using HMS.Communication;
using HMS.Communication.Abstractions;
using HMS.Communication.Hosting;
using HMS.Communication.Infrastructure.Observability;
using HMS.Communication.Persistence;
using HMS.Module.Lab.Infrastructure.Persistence;
using HMS.Realtime.Abstractions;
using HMS.SharedKernel.Ids;
using HMS.SharedServices.IdGeneration;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var cfg = builder.Configuration;

// Comm DB factory (as you already have)
builder.Services.AddDbContextFactory<CommunicationDbContext>(o =>
    o.UseSqlServer(cfg.GetConnectionString("HmsDb_Comm"),
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Comm")));

// ✅ Add the Lab module so ILabResultWriter is registered
builder.Services.AddLabModule(cfg);
// (optional) If your Lab module requires a context explicitly:
builder.Services.AddDbContext<LabDbContext>(o =>
    o.UseSqlServer(cfg.GetConnectionString("HmsDb_Lab"),
        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Lab")));

// --------------------Communication services--------------------
// Registers IProtocolAdapter, IRecordNormalizer, IMessageRouter, EventSink, drivers, etc.
builder.Services.AddCommunicationModule(cfg.GetConnectionString("HmsDb_Comm")!);

// business IDs (you already had this)
builder.Services.AddSingleton<IBusinessIdGenerator, TimeBasedBusinessIdGenerator>();

builder.Services.AddSingleton<ILabRealtime, NullLabRealtime>();

// worker options
builder.Services.Configure<AstmFileFeedHostOptions>(cfg.GetSection("Communication:Transport"));

// tracers you already wired (DbFrameTracer + File + Composite)
builder.Services.AddSingleton<DbFrameTracer>();
builder.Services.AddSingleton<IFrameTracer>(sp =>
{
    var folder = cfg["Communication:Trace:Folder"] ?? @"D:\HMS\comm\trace";
    Directory.CreateDirectory(folder);
    var db = sp.GetRequiredService<DbFrameTracer>();
    return new FileFrameTracer(Path.Combine(folder, "trace.log"));

});

// Finally register the composite (it will resolve every IFrameTracer except itself)
builder.Services.AddSingleton<IFrameTracer, CompositeFrameTracer>();

// background worker
builder.Services.AddHostedService<AstmFileFeedHost>();

await builder.Build().RunAsync();
