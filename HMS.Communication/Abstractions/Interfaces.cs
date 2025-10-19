namespace HMS.Communication.Abstractions;

public interface ITransport : IAsyncDisposable
{
    string Name { get; }
    Task OpenAsync(CancellationToken ct);
    Task CloseAsync(CancellationToken ct);
    Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct);
    //IAsyncEnumerable<ReadOnlyMemory<byte>> ReadFramesAsync(CancellationToken ct);
    Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct);
}

public interface IChannel : IAsyncDisposable
{
    DeviceRef Device { get; }
    ITransport Transport { get; }
    Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct);
    Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct);
}

public interface IProtocolAdapter
{
    string Protocol { get; }                  // "ASTM"
    // Simple worker loop for hosts
    Task RunAsync(IChannel channel, IRecordNormalizer normalizer, IMessageRouter router, IEventSink sink, IFrameTracer tracer, CancellationToken ct);

    // Optional low-level access
    Task<ParsedRecord?> ReadAsync(CancellationToken ct);
    Task SendOrderAsync(OrderDownload order, CancellationToken ct);
}

public interface IRecordNormalizer
{
    IEnumerable<NormalizedEvent> Normalize(ParsedRecord rec);
}

public interface IMessageRouter
{
    Task RouteAsync(NormalizedEvent ev, CancellationToken ct);
}

public interface IEventSink
{
    Task PersistsAsync(RawFrame frame, ParsedRecord? parsed, NormalizedEvent? ev, CancellationToken ct);
    Task PublishAsync(NormalizedEvent ev, CancellationToken ct);
}

public interface IFrameTracer
{
    Task TraceAsync(RawFrame frame, CancellationToken ct);
}

public interface IAstmOrderProvider
{
    Task<OrderDownload?> GetOrderForAccessionAsync(string accession, CancellationToken ct);
}
