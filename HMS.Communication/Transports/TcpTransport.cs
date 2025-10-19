using System.Net.Sockets;
using HMS.Communication.Abstractions;

namespace HMS.Communication.Transports;

public sealed class TcpTransport : ITransport
{
    private readonly TcpClient _client = new();
    private readonly string _host; private readonly int _port;
    public TcpTransport(string host, int port) { _host = host; _port = port; }
    public string Name => $"TCP:{_host}:{_port}";

    public async Task OpenAsync(CancellationToken ct) => await _client.ConnectAsync(_host, _port, ct);
    public async Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct) => await _client.GetStream().ReadAsync(buffer, ct);
    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct) => _client.GetStream().WriteAsync(buffer, ct).AsTask();
    public Task CloseAsync(CancellationToken ct) { _client.Close(); return Task.CompletedTask; }
    public ValueTask DisposeAsync() { _client.Dispose(); return ValueTask.CompletedTask; }
}
