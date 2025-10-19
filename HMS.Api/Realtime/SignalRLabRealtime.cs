// HMS.Api/Realtime/SignalRLabRealtime.cs
using HMS.Realtime.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace HMS.Api.Realtime;

public sealed class SignalRLabRealtime : ILabRealtime
{
    private readonly IHubContext<Hubs.LabHub> _hub;
    public SignalRLabRealtime(IHubContext<Hubs.LabHub> hub) => _hub = hub;

    public Task BroadcastAsync(HubEventDto dto, CancellationToken ct = default)
        => _hub.Clients.All.SendAsync("TraceEvent", dto, ct);

    public Task BroadcastToGroupAsync(string group, HubEventDto dto, CancellationToken ct = default)
        => _hub.Clients.Group(group).SendAsync("TraceEvent", dto, ct);
}
