// HMS.Communication/README.txt

1) Add connection string "HmsDb_Communication" to appsettings.json
2) In your API Program.cs call: services.AddCommunicationModule(Configuration);
3) Ensure Lab base URL in appsettings.json: "Lab:BaseUrl": "http://localhost:5000"
4) Configure transport:
   "Communication": {
      "Transport": { "Mode": "File" },
      "AstmFileFeed": { "Path": "bench/astm-feed.txt", "DeviceId": 1, "DeviceCode": "ROCHE1" },
      "Trace": { "Enabled": true, "File": "logs/comm-trace.log" }
   }
5) Run EF migrations for CommunicationDbContext, then start API.
6) To test, append ASTM lines to bench/astm-feed.txt (simulated frames). Normalized results will call Lab upsert endpoint.


1) Folder layout

HMS.Communication/
  Abstractions/
    ITransport.cs
    IProtocolAdapter.cs
    INormalizer.cs
    IMessageRouter.cs
    IEventSink.cs
    IFrameTracer.cs
    Models/...
  Infrastructure/
    Drivers/
      AnalyzerDriverFactory.cs
      AnalyzerDriverResolver.cs
      Analyzers/
        RocheCobasDriver.cs
        SysmexSuitDriver.cs
        FujiDryChemDriver.cs
    Pipeline/
      PipelineBuilder.cs
      PipelineOrchestrator.cs
    Protocols/
      ASTM/
        AstmAdapter.cs
        AstmNormalizer.cs
      HL7/
        Hl7MllpAdapter.cs
        Hl7OruNormalizer.cs
      Sysmex/
        SuitAdapter.cs
        SuitNormalizer.cs
      Fuji/
        FujiAsciiAdapter.cs
        FujiAsciiNormalizer.cs
    Routing/
      DefaultMessageRouter.cs
      ResultMapper.cs
      InstrumentMapResolver.cs
    Sinks/
      EventSink.cs
      TraceSink.cs
    Transports/
      SerialTransport.cs
      FileFeedTransport.cs
      TcpTransport.cs
      MllpTransport.cs
  Infrastructure/Persistence/
    Entities/...
    Configurations/...
    Seed/...


2) Core contracts (stable)

    // Abstractions/ITransport.cs
public interface ITransport : IAsyncDisposable
{
    string Name { get; }
    Task OpenAsync(CancellationToken ct);
    Task CloseAsync(CancellationToken ct);
    Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct);     // inbound
    Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct); // outbound if needed
}

// Abstractions/IProtocolAdapter.cs
public interface IProtocolAdapter
{
    string Protocol { get; } // "ASTM" | "HL7" | "SUIT" | "FUJI"
    IAsyncEnumerable<AdapterMessage> ReadMessagesAsync(ITransport transport, CancellationToken ct);
}

public sealed class AdapterMessage
{
    public DateTime At { get; init; } = DateTime.UtcNow;
    public string RawText { get; init; } = default!;
    public byte[] RawBytes { get; init; } = Array.Empty<byte>();
    public string? Accession { get; init; }
    public string? PatientId { get; init; }
    public string? HostTestCode { get; init; } // what the analyzer sent (numeric or token)
    public string? Value { get; init; }
    public string? Units { get; init; }
    public string? Flag { get; init; }
    public string? Status { get; init; } // F/C/P etc
    public IDictionary<string,string> Tags { get; } = new Dictionary<string,string>(StringComparer.OrdinalIgnoreCase);
}

// Abstractions/INormalizer.cs
public interface INormalizer
{
    NormalizedResult? Normalize(AdapterMessage msg);
}

public sealed class NormalizedResult
{
    public string? Accession { get; init; }
    public string? HostTestCode { get; init; }
    public string? Value { get; init; }
    public string? Units { get; init; }
    public string? Flag { get; init; }
    public DateTime At { get; init; }
    public string SourceProtocol { get; init; } = default!;
    public string Raw { get; init; } = default!;
}

// Abstractions/IMessageRouter.cs
public interface IMessageRouter
{
    Task RouteAsync(long deviceId, NormalizedResult r, CancellationToken ct);
}

// Abstractions/IEventSink.cs
public interface IEventSink
{
    Task PersistInboundAsync(long deviceId, string transportName, string rawText, byte[] rawBytes, CancellationToken ct);
    Task PersistResultAsync(long deviceId, NormalizedResult r, CancellationToken ct);
}

// Abstractions/IFrameTracer.cs, ITraceBroadcaster.cs are as you already have


3) Transports (add TCP/MLLP)

// Transports/TcpTransport.cs  (simple TCP client)
public sealed class TcpTransport : ITransport
{
    private readonly string _host; private readonly int _port;
    private TcpClient? _client; private NetworkStream? _stream;
    public string Name => $"TCP:{_host}:{_port}";
    public TcpTransport(string host, int port){ _host=host; _port=port; }

    public async Task OpenAsync(CancellationToken ct){
        _client = new TcpClient();
        await _client.ConnectAsync(_host,_port, ct);
        _stream = _client.GetStream();
    }
    public Task CloseAsync(CancellationToken ct){ _stream?.Dispose(); _client?.Close(); return Task.CompletedTask; }
    public async Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct){
        if (_stream==null) return 0;
        return await _stream.ReadAsync(buffer, ct);
    }
    public async Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct){
        if (_stream==null) return;
        await _stream.WriteAsync(buffer, ct);
    }
    public ValueTask DisposeAsync(){ _stream?.Dispose(); _client?.Dispose(); return ValueTask.CompletedTask; }
}

// Transports/MllpTransport.cs (HL7 MLLP wrapper over TCP)
public sealed class MllpTransport : ITransport
{
    private readonly TcpTransport _tcp;
    public string Name => $"MLLP:{_tcp.Name}";
    private const byte SB = 0x0B; // VT
    private const byte EB = 0x1C; // FS
    private const byte CR = 0x0D;

    public MllpTransport(string host,int port){ _tcp = new TcpTransport(host, port); }
    public Task OpenAsync(CancellationToken ct) => _tcp.OpenAsync(ct);
    public Task CloseAsync(CancellationToken ct)=> _tcp.CloseAsync(ct);

    public async Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct){
        // Read one full MLLP frame into buffer (SB .... EB CR)
        using var ms = new MemoryStream();
        var temp = new byte[4096];
        // wait for SB
        while(true){
            var n = await _tcp.ReadAsync(temp, ct);
            if(n<=0) return 0;
            int i=0;
            for(;i<n;i++){
                if(temp[i]==SB){ i++; // start recording
                    for(;i<n;i++){ if(temp[i]==EB) break; ms.WriteByte(temp[i]); }
                    // ensure CR after EB
                    if (i<n-1 && temp[i+1]==CR) { /* ok */ }
                    var payload = ms.ToArray();
                    payload.CopyTo(buffer[..payload.Length].Span);
                    return payload.Length;
                }
            }
        }
    }

    public async Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct){
        // Wrap payload in SB ... EB CR
        var ms = new MemoryStream();
        ms.WriteByte(SB);
        await ms.WriteAsync(buffer, ct);
        ms.WriteByte(EB); ms.WriteByte(CR);
        await _tcp.WriteAsync(ms.ToArray(), ct);
    }
    public ValueTask DisposeAsync()=>_tcp.DisposeAsync();
}


4) Adapters (ASTM, HL7/MLLP, Sysmex SUIT, Fuji ASCII)
ASTM

// Protocols/ASTM/AstmAdapter.cs
public sealed class AstmAdapter : IProtocolAdapter
{
    public string Protocol => "ASTM";
    public async IAsyncEnumerable<AdapterMessage> ReadMessagesAsync(ITransport transport, [EnumeratorCancellation] CancellationToken ct)
    {
        var buf = new byte[65536];
        // naive: read all (file/serial) — you likely already have better chunking
        var n = await transport.ReadAsync(buf, ct);
        if (n <= 0) yield break;

        var ascii = Encoding.ASCII.GetString(buf, 0, n);
        // Very simple: split result records R|...; one msg per R
        foreach (var line in ascii.Split('\r'))
        {
            if (!line.StartsWith("R|")) continue;
            // R|1|^^^{TEST}^1|VALUE|UNITS|FLAG||STATUS
            var f = line.Split('|');
            var field3 = f.Length>2 ? f[2] : "";
            var id = field3.Split('^');
            var test = id.Length>=4 ? id[3] : null;
            var value = f.Length>3 ? f[3] : null;
            var units = f.Length>4 ? f[4] : null;
            var flag  = f.Length>5 ? f[5] : null;
            var status= f.Length>7 ? f[7] : null;

            yield return new AdapterMessage {
                RawText = line, RawBytes = Encoding.ASCII.GetBytes(line+"\r"),
                HostTestCode = test, Value = value, Units = units, Flag = flag, Status = status
            };
        }
    }
}


HL7 ORU via MLLP

// Protocols/HL7/Hl7MllpAdapter.cs
public sealed class Hl7MllpAdapter : IProtocolAdapter
{
    public string Protocol => "HL7";
    public async IAsyncEnumerable<AdapterMessage> ReadMessagesAsync(ITransport transport, [EnumeratorCancellation] CancellationToken ct)
    {
        var buf = new byte[65536];
        var n = await transport.ReadAsync(buf, ct);
        if (n<=0) yield break;
        var text = Encoding.ASCII.GetString(buf,0,n);
        // split into messages by MSH; naive
        var messages = text.Split("\rMSH").Select((m,i)=> i==0? m : "MSH"+m);
        foreach (var msg in messages)
        {
            if (string.IsNullOrWhiteSpace(msg)) continue;
            // Extract OBX segments
            foreach (var line in msg.Split('\r'))
            {
                if (!line.StartsWith("OBX|")) continue;
                // OBX|n|ST|TEST^..|...|VALUE|UNITS|...|FLAG|...
                var f = line.Split('|');
                var id  = f.Length>3 ? f[3] : "";
                var idParts = id.Split('^');
                var test = idParts.Length>0? idParts[0] : null;
                var value= f.Length>5 ? f[5] : null;
                var unit = f.Length>6 ? f[6] : null;
                var flag = f.Length>8 ? f[8] : null;

                yield return new AdapterMessage {
                    RawText = line, RawBytes = Encoding.ASCII.GetBytes(line+"\r"),
                    HostTestCode = test, Value = value, Units = unit, Flag = flag
                };
            }
        }
    }
}


Sysmex SUIT (minimal)
// Protocols/Sysmex/SuitAdapter.cs
public sealed class SuitAdapter : IProtocolAdapter
{
    public string Protocol => "SUIT";

    public async IAsyncEnumerable<AdapterMessage> ReadMessagesAsync(ITransport transport, [EnumeratorCancellation] CancellationToken ct)
    {
        var buf = new byte[65536];
        var n = await transport.ReadAsync(buf, ct);
        if (n<=0) yield break;
        var text = Encoding.ASCII.GetString(buf,0,n);

        // Minimal SUIT: assume lines like  "WBC=9.56|10^9/L|N"
        foreach (var raw in text.Split(new[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries))
        {
            var line = raw.Trim();
            if (line.StartsWith("#") || line.Length<3) continue;

            // try key=value|unit|flag
            var code = line; string? value=null, unit=null, flag=null;
            var eq = line.IndexOf('=');
            if (eq>0){
                code = line[..eq];
                var rest = line[(eq+1)..];
                var parts = rest.Split('|');
                value = parts.ElementAtOrDefault(0);
                unit  = parts.ElementAtOrDefault(1);
                flag  = parts.ElementAtOrDefault(2);
            }

            yield return new AdapterMessage{
                RawText = line, RawBytes = Encoding.ASCII.GetBytes(line+"\r"),
                HostTestCode = code, Value = value, Units = unit, Flag = flag
            };
        }
    }
}


Fuji ASCII (minimal)
// Protocols/Fuji/FujiAsciiAdapter.cs
public sealed class FujiAsciiAdapter : IProtocolAdapter
{
    public string Protocol => "FUJI";
    public async IAsyncEnumerable<AdapterMessage> ReadMessagesAsync(ITransport transport, [EnumeratorCancellation] CancellationToken ct)
    {
        var buf = new byte[65536];
        var n = await transport.ReadAsync(buf, ct);
        if (n<=0) yield break;
        var text = Encoding.ASCII.GetString(buf,0,n);

        // Minimal: lines like "GLU,104,mg/dL,N"
        foreach (var raw in text.Split(new[]{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries))
        {
            var line = raw.Trim();
            if (line.StartsWith("#") || line.Length<3) continue;

            var cols = line.Split(',');
            var code = cols.ElementAtOrDefault(0);
            var val  = cols.ElementAtOrDefault(1);
            var unit = cols.ElementAtOrDefault(2);
            var flag = cols.ElementAtOrDefault(3);

            yield return new AdapterMessage{
                RawText = line, RawBytes = Encoding.ASCII.GetBytes(line+"\r"),
                HostTestCode = code, Value = val, Units = unit, Flag = flag
            };
        }
    }
}



5) Normalizers (thin wrappers)
// Protocols/ASTM/AstmNormalizer.cs
public sealed class AstmNormalizer : INormalizer
{
    public NormalizedResult? Normalize(AdapterMessage m) => new(){
        Accession = m.Accession, HostTestCode = m.HostTestCode,
        Value = m.Value, Units = m.Units, Flag = m.Flag, At = m.At,
        SourceProtocol = "ASTM", Raw = m.RawText
    };
}

// Protocols/HL7/Hl7OruNormalizer.cs
public sealed class Hl7OruNormalizer : INormalizer
{
    public NormalizedResult? Normalize(AdapterMessage m) => new(){
        Accession = m.Accession, HostTestCode = m.HostTestCode,
        Value = m.Value, Units = m.Units, Flag = m.Flag, At = m.At,
        SourceProtocol = "HL7", Raw = m.RawText
    };
}

// Protocols/Sysmex/SuitNormalizer.cs
public sealed class SuitNormalizer : INormalizer
{
    public NormalizedResult? Normalize(AdapterMessage m) => new(){
        Accession = m.Accession, HostTestCode = m.HostTestCode,
        Value = m.Value, Units = m.Units, Flag = m.Flag, At = m.At,
        SourceProtocol = "SUIT", Raw = m.RawText
    };
}

// Protocols/Fuji/FujiAsciiNormalizer.cs
public sealed class FujiAsciiNormalizer : INormalizer
{
    public NormalizedResult? Normalize(AdapterMessage m) => new(){
        Accession = m.Accession, HostTestCode = m.HostTestCode,
        Value = m.Value, Units = m.Units, Flag = m.Flag, At = m.At,
        SourceProtocol = "FUJI", Raw = m.RawText
    };
}


6) Router + Sink

// Routing/InstrumentMapResolver.cs
public interface IInstrumentMapResolver
{
    Task<(long? labTestId, string? labCode)> ResolveAsync(long deviceId, string hostTestCode, CancellationToken ct);
}

public sealed class InstrumentMapResolver : IInstrumentMapResolver
{
    private readonly LabDbContext _lab;
    public InstrumentMapResolver(LabDbContext lab) => _lab = lab;

    public async Task<(long?, string?)> ResolveAsync(long deviceId, string host, CancellationToken ct)
    {
        var m = await _lab.InstrumentTestMaps
            .Where(x => x.DeviceId==deviceId && x.InstrumentTestCode==host && !x.IsDeleted)
            .Select(x => new { x.LabTestId, x.LabTestCode })
            .FirstOrDefaultAsync(ct);

        return m is null ? (null, null) : (m.LabTestId, m.LabTestCode);
    }
}

// Routing/DefaultMessageRouter.cs
public sealed class DefaultMessageRouter : IMessageRouter
{
    private readonly IInstrumentMapResolver _maps;
    private readonly IEventSink _sink;
    public DefaultMessageRouter(IInstrumentMapResolver maps, IEventSink sink){ _maps=maps; _sink=sink; }

    public async Task RouteAsync(long deviceId, NormalizedResult r, CancellationToken ct)
    {
        // Resolve test mapping
        var (labTestId, labCode) = await _maps.ResolveAsync(deviceId, r.HostTestCode ?? "", ct);
        // Persist result (your sink can write to CommEvents + post to Lab)
        await _sink.PersistResultAsync(deviceId, r with { /* keep as-is */ }, ct);
        // TODO: call Lab integration (post to LabResults based on accession + labTestId)
    }
}

// Sinks/EventSink.cs
public sealed class EventSink : IEventSink
{
    private readonly CommunicationDbContext _comm;
    private readonly ILabRealtime _labHub;
    public EventSink(CommunicationDbContext comm, ILabRealtime labHub){ _comm=comm; _labHub=labHub; }

    public async Task PersistInboundAsync(long deviceId, string transport, string rawText, byte[] rawBytes, CancellationToken ct)
    {
        _comm.CommMessageInbound.Add(new CommMessageInbound{
            DeviceId = deviceId, Direction="IN", Transport=transport,
            Ascii = rawText, Bytes = rawBytes, At = DateTime.UtcNow,
            BusinessNo = Guid.NewGuid().ToString("N")
        });
        await _comm.SaveChangesAsync(ct);
    }

    public async Task PersistResultAsync(long deviceId, NormalizedResult r, CancellationToken ct)
    {
        _comm.CommEvents.Add(new CommEvent{
            DeviceId = deviceId, Kind="Result",
            Accession = r.Accession, LabTestCode = r.HostTestCode,
            Value = r.Value, Units=r.Units, Flag=r.Flag, At = r.At,
            EventId = Guid.NewGuid().ToString("N")
        });
        await _comm.SaveChangesAsync(ct);
        // optional: notify lab hub/UI
        await _labHub.ResultPosted(new(){ Accession = r.Accession, Test = r.HostTestCode, Value = r.Value, Units = r.Units, Flag = r.Flag });
    }
}


7) Pipeline builder (choose adapter/normalizer per profile)

// Pipeline/PipelineBuilder.cs
public interface IPipeline
{
    Task RunAsync(long deviceId, ITransport transport, CancellationToken ct);
}

public sealed class PipelineBuilder
{
    private readonly IServiceProvider _sp;
    public PipelineBuilder(IServiceProvider sp) => _sp = sp;

    public IPipeline Build(string protocol)
    {
        IProtocolAdapter adapter = protocol.ToUpperInvariant() switch {
            "ASTM" => ActivatorUtilities.CreateInstance<AstmAdapter>(_sp),
            "HL7"  => ActivatorUtilities.CreateInstance<Hl7MllpAdapter>(_sp),
            "SUIT" => ActivatorUtilities.CreateInstance<SuitAdapter>(_sp),
            "FUJI" => ActivatorUtilities.CreateInstance<FujiAsciiAdapter>(_sp),
            _ => ActivatorUtilities.CreateInstance<AstmAdapter>(_sp)
        };
        INormalizer norm = protocol.ToUpperInvariant() switch {
            "ASTM" => ActivatorUtilities.CreateInstance<AstmNormalizer>(_sp),
            "HL7"  => ActivatorUtilities.CreateInstance<Hl7OruNormalizer>(_sp),
            "SUIT" => ActivatorUtilities.CreateInstance<SuitNormalizer>(_sp),
            "FUJI" => ActivatorUtilities.CreateInstance<FujiAsciiNormalizer>(_sp),
            _ => ActivatorUtilities.CreateInstance<AstmNormalizer>(_sp)
        };
        var router = _sp.GetRequiredService<IMessageRouter>();
        var sink   = _sp.GetRequiredService<IEventSink>();
        var tracer = _sp.GetRequiredService<IFrameTracer>();

        return new PipelineOrchestrator(adapter, norm, router, sink, tracer);
    }
}

// Pipeline/PipelineOrchestrator.cs
public sealed class PipelineOrchestrator : IPipeline
{
    private readonly IProtocolAdapter _adapter;
    private readonly INormalizer _norm;
    private readonly IMessageRouter _router;
    private readonly IEventSink _sink;
    private readonly IFrameTracer _trace;

    public PipelineOrchestrator(IProtocolAdapter adapter, INormalizer norm, IMessageRouter router, IEventSink sink, IFrameTracer trace)
        { _adapter=adapter; _norm=norm; _router=router; _sink=sink; _trace=trace; }

    public async Task RunAsync(long deviceId, ITransport transport, CancellationToken ct)
    {
        await foreach (var msg in _adapter.ReadMessagesAsync(transport, ct))
        {
            await _sink.PersistInboundAsync(deviceId, transport.Name, msg.RawText, msg.RawBytes, ct);
            await _trace.TraceAsync(new RawFrame{
                At = msg.At, Dir="IN",
                Device = new RawDevice{ Code = deviceId.ToString(), Name = "" },
                Transport = transport.Name, Ascii = msg.RawText, Bytes = msg.RawBytes
            }, ct);

            var r = _norm.Normalize(msg);
            if (r != null) await _router.RouteAsync(deviceId, r, ct);
        }
    }
}


8) Program.cs registration (DI + config)
// Program.cs (Communication API or Host)
builder.Services.AddSingleton<IMessageRouter, DefaultMessageRouter>();
builder.Services.AddSingleton<IEventSink, EventSink>();
builder.Services.AddSingleton<IInstrumentMapResolver, InstrumentMapResolver>();

// tracers you already have
builder.Services.AddSingleton<IFrameTracer, SignalRFrameTracer>();
builder.Services.AddSingleton<IFrameTracer, CompositeFrameTracer>();

builder.Services.AddSingleton<PipelineBuilder>();

// Drivers factory/resolver already in your project:
builder.Services.AddSingleton<IAnalyzerDriverFactory, AnalyzerDriverFactory>();
builder.Services.AddSingleton<IAnalyzerDriverResolver, AnalyzerDriverResolver>();



Config sample (appsettings.json)
{
  "Communication": {
    "Trace": { "Enabled": true, "Folder": "D:\\HMS\\comm\\trace" },
    "Devices": [
      { "Code":"COBAS-C311-01","Protocol":"ASTM","Transport":"File","Path":"D:\\HMS\\comm\\in\\astm.txt" },
      { "Code":"SYSMEX-XP300-01","Protocol":"SUIT","Transport":"Serial","Port":"COM6","Baud":9600,"Parity":"None","DataBits":8,"StopBits":1 },
      { "Code":"FUJI-NX500-01","Protocol":"FUJI","Transport":"Serial","Port":"COM7","Baud":9600,"Parity":"None","DataBits":8,"StopBits":1 },
      { "Code":"HL7-LIS-IN","Protocol":"HL7","Transport":"MLLP","Host":"127.0.0.1","Port":2575 }
    ]
  }
}


9) A tiny “bench send” per protocol (for testing)

You already have a good bench send for ASTM. Add similar minimal ones:

// HL7 ORU bench (wraps MLLP and sends one OBX)
app.MapPost("/api/comm/bench/hl7/oru", async ([FromBody] Hl7BenchDto dto, CancellationToken ct) =>
{
    var mllp = new MllpTransport(dto.Host, dto.Port);
    await mllp.OpenAsync(ct);
    var hl7 = $"MSH|^~\\&|ANALYZER|LAB|||{DateTime.UtcNow:yyyyMMddHHmmss}||ORU^R01|1|P|2.3\r" +
              $"PID|1||{dto.PatientId}||DOE^JOHN\r" +
              $"OBR|1|{dto.Accession}|{dto.Accession}|||{DateTime.UtcNow:yyyyMMddHHmmss}\r" +
              $"OBX|1|ST|{dto.TestCode}^||{dto.Value}|{dto.Unit}|{dto.Flag}|||F\r";
    await mllp.WriteAsync(Encoding.ASCII.GetBytes(hl7), ct);
    await mllp.CloseAsync(ct);
    return Results.Ok();
});
public record Hl7BenchDto(string Host, int Port, string Accession, string PatientId, string TestCode, string Value, string Unit, string? Flag);


2) Tiny selector change in the worker (one small edit)

Where your file-feed/serial host currently hardcodes ASTM pieces, switch to the PipelineBuilder:

// before: you manually wired AstmAdapter + AstmRecordNormalizer, etc.

// after: select by device profile
var device = /* resolve CommDevice by code or id */;
var protocol = device.AnalyzerProfile?.Protocol ?? "ASTM";

var pipeline = sp.GetRequiredService<PipelineBuilder>().Build(protocol);
await using var transport = /* create transport from your current config (File/Serial/TCP/MLLP) */;
await transport.OpenAsync(ct);
await pipeline.RunAsync(device.CommDeviceId, transport, ct);

