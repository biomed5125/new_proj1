using HMS.Communication.Application.Services;
using HMS.Communication.Domain.Abstractions;
using HMS.Communication.Domain.CommEntities;
using HMS.Communication.Infrastructure.Persistence;
using HMS.Communication.Infrastructure.Protocols;
using HMS.Communication.Infrastructure.Transports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

namespace HMS.Communication.Hosting;

public sealed class DeviceHostedService : BackgroundService
{
    private readonly IServiceProvider _sp;

    public DeviceHostedService(IServiceProvider sp) => _sp = sp;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _sp.CreateScope();
        var comm = scope.ServiceProvider.GetRequiredService<CommunicationDbContext>();
        var router = scope.ServiceProvider.GetRequiredService<IMessageRouter>();
        var ingest = scope.ServiceProvider.GetRequiredService<ResultIngestService>();
        var dispatch = scope.ServiceProvider.GetRequiredService<OrderDispatchService>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var devices = await comm.Devices.Where(d => d.Enabled).ToListAsync(stoppingToken);
            foreach (var dev in devices)
            {
                _ = Task.Run(() => RunDeviceLoopAsync(dev, router, ingest, dispatch, stoppingToken));
            }
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // rescan
        }
    }

    private async Task RunDeviceLoopAsync(Device dev, IMessageRouter router, ResultIngestService ingest, OrderDispatchService dispatch, CancellationToken ct)
    {
        try
        {
            // Order dispatch (host-initiated)
            await dispatch.DispatchPendingAsync(dev.DeviceId, ct);

            // Result collection (instrument-initiated ENQ)
            var sc = JsonSerializer.Deserialize<SerialCfg>(dev.TransportSettingsJson) ?? new();
            var parity = Enum.Parse<System.IO.Ports.Parity>(sc.Parity, true);
            await using var transport = new SerialTransport(sc.PortName, sc.Baud, parity, sc.DataBits, (System.IO.Ports.StopBits)sc.StopBits);
            await transport.OpenAsync(ct);
            await using var session = new AstmSession(transport);

            await foreach (var records in session.ReadBatchesAsync(ct))
            {
                try
                {
                    var driver = router.Resolve(dev.DriverKey);
                    await driver.OnIncomingRecordsAsync(dev.DeviceId, records, ct);  // device-specific hook (optional)
                    await ingest.IngestAsync(dev.DeviceId, records, ct);            // generic ingest → Lab
                }
                catch (Exception ex) { /* log & continue */ }
            }
        }
        catch (Exception)
        {
            // log dev error; backoff
            await Task.Delay(2000, ct);
        }
    }

    private sealed class SerialCfg { public string PortName { get; set; } = "COM1"; public int Baud { get; set; } = 9600; public string Parity { get; set; } = "None"; public int DataBits { get; set; } = 8; public int StopBits { get; set; } = 1; }
}
