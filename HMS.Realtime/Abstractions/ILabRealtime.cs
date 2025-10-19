namespace HMS.Realtime.Abstractions;

public sealed record HubEventDto(
    DateTimeOffset At,
    long DeviceId,
    string Kind,              // "ResultPosted" | "Trace" | "DecodeFail" | ...
    string? Accession,
    string? InstrumentCode,
    string? Value,
    string? Units,
    string? Flag
);

public interface ILabRealtime
{
    Task BroadcastAsync(HubEventDto dto, CancellationToken ct = default);
    Task BroadcastToGroupAsync(string group, HubEventDto dto, CancellationToken ct = default);
}
