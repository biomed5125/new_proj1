using HMS.Communication.Abstractions;

namespace HMS.Communication.Pipeline;

/// <summary>
/// Simple dispatcher that lets adapters push parsed records
/// through normalization → routing → persistence.
/// </summary>
public sealed class CommRouter
{
    private readonly IRecordNormalizer _normalizer;
    private readonly IMessageRouter _router;
    private readonly IEventSink _sink;

    public CommRouter(IRecordNormalizer normalizer, IMessageRouter router, IEventSink sink)
    {
        _normalizer = normalizer;
        _router = router;
        _sink = sink;
    }

    /// <summary>
    /// Handle one parsed record (from adapter).
    /// </summary>
    public async Task HandleAsync(RawFrame frame, ParsedRecord rec, IFrameTracer tracer, CancellationToken ct)
    {
        // Persist raw+parsed
        await _sink.PersistsAsync(frame, rec, null, ct);

        // Normalize → route
        foreach (var ev in _normalizer.Normalize(rec))
        {
            await _sink.PersistsAsync(frame, rec, ev, ct);
            await _sink.PublishAsync(ev, ct);
            await _router.RouteAsync(ev, ct);
        }
    }
}
