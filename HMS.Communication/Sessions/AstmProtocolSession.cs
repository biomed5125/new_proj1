using HMS.Communication.Abstractions;

namespace HMS.Communication.Sessions;

public sealed class AstmProtocolSession : IChannel
{
    public DeviceRef Device { get; }
    public ITransport Transport { get; }

    public AstmProtocolSession(DeviceRef device, ITransport transport)
    {
        Device = device;
        Transport = transport;
    }

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct)
        => Transport.ReadAsync(buffer, ct);

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct)
        => Transport.WriteAsync(buffer, ct);

    public ValueTask DisposeAsync() => Transport.DisposeAsync();
}
