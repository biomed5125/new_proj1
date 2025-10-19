using System.IO.Ports;
using HMS.Communication.Abstractions;

namespace HMS.Communication.Transports;

public sealed class SerialTransport : ITransport
{
    private readonly SerialPort _port;
    public string Name => $"Serial:{_port.PortName}";

    public SerialTransport(string portName, int baud, Parity parity, int dataBits, StopBits stopBits)
    {
        _port = new SerialPort(portName, baud, parity, dataBits, stopBits) { Handshake = Handshake.None, NewLine = "\r\n" };
    }

    public Task OpenAsync(CancellationToken ct) { _port.Open(); return Task.CompletedTask; }
    public Task CloseAsync(CancellationToken ct) { _port.Close(); return Task.CompletedTask; }

    public Task<int> ReadAsync(Memory<byte> buffer, CancellationToken ct)
    {
        var arr = new byte[buffer.Length];
        var n = _port.Read(arr, 0, arr.Length);
        if (n > 0) new ReadOnlySpan<byte>(arr, 0, n).CopyTo(buffer.Span);
        return Task.FromResult(n);
    }

    public Task WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken ct)
    {
        // SerialPort in some TFMs lacks Span overloads — use array copy
        var arr = buffer.ToArray();
        _port.Write(arr, 0, arr.Length);
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync() { _port.Dispose(); return ValueTask.CompletedTask; }
}
