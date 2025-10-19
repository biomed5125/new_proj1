// HMS.Api/Hubs/LabHub.cs
using HMS.Realtime.Abstractions;
using Microsoft.AspNetCore.SignalR;

namespace HMS.Api.Hubs;

public interface ILabHubClient
{
    Task TraceEvent(HubEventDto dto);
}

public sealed class LabHub : Hub<ILabHubClient>
{
    public Task JoinDeviceGroup(string group) => Groups.AddToGroupAsync(Context.ConnectionId, group);
    public Task LeaveDeviceGroup(string group) => Groups.RemoveFromGroupAsync(Context.ConnectionId, group);
}
