using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using HMS.Communication.Abstractions;
using HMS.Communication.Pipeline;
using HMS.Communication.Sessions;
using HMS.Communication.Transports;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HMS.Communication.Hosting;

public sealed class AstmSerialHostOptions
{
    public string PortName { get; set; } = "COM3";
    public int Baud { get; set; } = 9600;
    public Parity Parity { get; set; } = Parity.None;
    public int DataBits { get; set; } = 8;
    public StopBits StopBits { get; set; } = StopBits.One;
    public long DeviceId { get; set; } = 1;
    public string DeviceCode { get; set; } = "ROCHE1";
}

public sealed class AstmSerialHost : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly AstmSerialHostOptions _opt;
    private readonly ILogger<AstmSerialHost> _log;

    public AstmSerialHost(
        IServiceProvider sp,
        IOptions<AstmSerialHostOptions> opt,
        ILogger<AstmSerialHost> log)
    {
        _sp = sp;
        _opt = opt.Value;
        _log = log;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var transport = new SerialTransport(_opt.PortName, _opt.Baud, _opt.Parity, _opt.DataBits, _opt.StopBits);
        await transport.OpenAsync(stoppingToken);
        await using var _ = transport;

        _log.LogInformation("ASTM Serial running on {port} for device {dev}", _opt.PortName, _opt.DeviceCode);

        var device = new DeviceRef(_opt.DeviceId, _opt.DeviceCode);
        var channel = new AstmProtocolSession(device, transport);

        using var scope = _sp.CreateScope();
        var adapter = scope.ServiceProvider.GetRequiredService<IProtocolAdapter>();
        var normalizer = scope.ServiceProvider.GetRequiredService<IRecordNormalizer>();
        var msgRouter = scope.ServiceProvider.GetRequiredService<IMessageRouter>();
        var sink = scope.ServiceProvider.GetRequiredService<IEventSink>();
        var tracer = scope.ServiceProvider.GetRequiredService<IFrameTracer>();
        var router = scope.ServiceProvider.GetRequiredService<CommRouter>();

        await adapter.RunAsync(channel, normalizer, msgRouter, sink, tracer, stoppingToken);
    }
}
