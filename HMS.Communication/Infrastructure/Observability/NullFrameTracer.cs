using HMS.Communication.Abstractions;

namespace HMS.Communication.Infrastructure.Observability;

public sealed class NullFrameTracer : IFrameTracer
{
    public Task TraceAsync(RawFrame frame, CancellationToken ct) => Task.CompletedTask;
}
