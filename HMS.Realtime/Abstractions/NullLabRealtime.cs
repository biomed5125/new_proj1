// HMS.Realtime/Abstractions/NullLabRealtime.cs
using HMS.Realtime.Abstractions;

namespace HMS.Realtime.Abstractions;

public sealed class NullLabRealtime : ILabRealtime
{
    public Task BroadcastAsync(HubEventDto dto, CancellationToken ct = default) => Task.CompletedTask;
    public Task BroadcastToGroupAsync(string group, HubEventDto dto, CancellationToken ct = default) => Task.CompletedTask;
}
