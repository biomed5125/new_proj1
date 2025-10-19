using HMS.Communication.Abstractions;

namespace HMS.Communication.Services;

public sealed class OrderDispatchService
{
    public Task HandleAsync(InboundEnvelope env, CancellationToken ct)
    {
        // If you later want to send orders to device (host->instrument), build frames here.
        return Task.CompletedTask;
    }
}
