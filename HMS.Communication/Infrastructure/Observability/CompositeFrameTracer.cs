using HMS.Communication.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace HMS.Communication.Infrastructure.Observability;

public sealed class CompositeFrameTracer : IFrameTracer
{
    private readonly IServiceProvider _sp;
    private IReadOnlyList<IFrameTracer>? _sinks;   // cached after first resolve

    public CompositeFrameTracer(IServiceProvider sp) => _sp = sp;

    public async Task TraceAsync(RawFrame frame, CancellationToken ct)
    {
        // Lazily resolve *all* IFrameTracer implementations and exclude myself.
        // This avoids circular DI while letting you register multiple tracers.
        _sinks ??= _sp.GetServices<IFrameTracer>()
                      .Where(t => t is not CompositeFrameTracer)
                      .ToList();

        // Fan-out to all sinks (file, SignalR, etc.)
        foreach (var t in _sinks)
            await t.TraceAsync(frame, ct).ConfigureAwait(false);
    }
}
