using HMS.Api.Hubs;
using HMS.Communication.Abstractions;
using Microsoft.AspNetCore.SignalR;
using System.Text;

namespace HMS.Api.Observability;
public interface ITraceBroadcaster
{
    Task PublishAsync(TraceDto dto, CancellationToken ct);
}

public readonly record struct TraceDto(
    DateTimeOffset AtUtc,
    string Device,
    string Direction,
    string Transport,
    string Frame);
public sealed class SignalRFrameTracer(ITraceBroadcaster broadcaster) : IFrameTracer
{
    public async Task TraceAsync(RawFrame frame, CancellationToken ct)
    {
        var ascii = frame.Ascii ??
                    (frame.Bytes is { Length: > 0 } ? Encoding.ASCII.GetString(frame.Bytes) : string.Empty);

        var dto = new TraceDto(
            AtUtc: frame.At,
            Device: frame.Device.Code,
            Direction: frame.Dir == FrameDirection.Rx ? "IN" : "OUT",
            Transport: frame.Transport ?? "",
            Frame: ascii.Replace("\r", "\\r").Replace("\n", "\\n")
        );

        await broadcaster.PublishAsync(dto, ct);
    }
}

public sealed class TraceBroadcaster(IHubContext<CommTraceHub> hub) : ITraceBroadcaster
{
    public Task PublishAsync(TraceDto dto, CancellationToken ct)
        => hub.Clients.All.SendAsync("trace", dto, ct);
}
