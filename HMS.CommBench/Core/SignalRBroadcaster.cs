namespace HMS.CommBench.Core;

public sealed class SignalRBroadcaster
{
    // TODO: add Microsoft.AspNetCore.SignalR.Client and implement real hub push.
    private readonly bool _enabled = false;

    public SignalRBroadcaster(string? hubUrl = null)
    {
        _enabled = !string.IsNullOrWhiteSpace(hubUrl);
    }

    public Task PublishAsync(DateTime at, string dir, string text, CancellationToken ct = default)
    {
        // no-op for now
        return Task.CompletedTask;
    }
}
