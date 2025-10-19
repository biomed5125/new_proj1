// HMS.Communication.Host/Program.cs
using HMS.Communication;
using HMS.Communication.Abstractions;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Infrastructure.Persistence.Entities;
using HMS.Communication.Options;
using HMS.Module.Lab;
using HMS.Module.Lab.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

var builder = Host.CreateApplicationBuilder(args);

// config & logging
var cfg = builder.Configuration;
builder.Services.AddLogging(o => o.AddConsole());

// register Lab DB (so router can write LabResults) and Comm module
builder.Services.AddLabModule(cfg);               // your existing AddLabModule(...)
builder.Services.AddCommunicationModule(cfg);     // new comm module

// background loop
builder.Services.AddHostedService<CommWorker>();

var host = builder.Build();

// ensure schemas and seed devices from config
using (var scope = host.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<LabDbContext>().Database.MigrateAsync();
    var comm = scope.ServiceProvider.GetRequiredService<CommDbContext>();
    await comm.Database.MigrateAsync();

    var opts = scope.ServiceProvider.GetRequiredService<IOptions<DeviceOptions>>().Value;
    if (opts.Devices.Count > 0)
    {
        // upsert devices from config
        foreach (var d in opts.Devices)
        {
            var existing = await comm.Devices.FirstOrDefaultAsync(x => x.CommDeviceId == d.CommDeviceId);
            if (existing is null)
            {
                comm.Devices.Add(new CommDevice
                {
                    CommDeviceId = d.CommDeviceId,
                    Name = d.Name,
                    Model = d.Model,
                    Protocol = d.Protocol,
                    Transport = d.Transport,
                    TransportSettingsJson = d.TransportSettingsJson,
                    SendOrders = d.SendOrders,
                    ReceiveResults = d.ReceiveResults,
                    IsEnabled = d.IsEnabled,
                    CreatedAt = DateTime.UtcNow
                });
            }
            else
            {
                existing.Name = d.Name; existing.Model = d.Model; existing.Protocol = d.Protocol;
                existing.Transport = d.Transport; existing.TransportSettingsJson = d.TransportSettingsJson;
                existing.SendOrders = d.SendOrders; existing.ReceiveResults = d.ReceiveResults; existing.IsEnabled = d.IsEnabled;
            }
        }
        await comm.SaveChangesAsync();
    }
}

await host.RunAsync();

public sealed class CommWorker : BackgroundService
{
    private readonly IMessageRouter _router;
    private readonly CommDbContext _db;

    public CommWorker(IMessageRouter router, CommDbContext db)
    { _router = router; _db = db; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var devices = await _db.Devices.Where(d => d.IsEnabled).ToListAsync(stoppingToken);
            foreach (var d in devices)
            {
                await _router.PushOrdersAsync(d.CommDeviceId, stoppingToken);
                await _router.PullResultsAsync(d.CommDeviceId, stoppingToken);
            }
            await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken);
        }
    }
}
