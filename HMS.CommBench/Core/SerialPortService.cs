using System.IO.Ports;

namespace HMS.CommBench.Core;

public sealed class SerialPortService : IDisposable
{
    private readonly SerialPort _port = new();

    public event EventHandler<byte[]>? BytesReceived;

    public string? PortName
    {
        get => _port.PortName;
        set { if (!_port.IsOpen) _port.PortName = value ?? ""; }
    }

    public void Configure(int baud = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
    {
        if (_port.IsOpen) throw new InvalidOperationException("Close port before reconfiguring.");
        _port.BaudRate = baud; _port.Parity = parity; _port.DataBits = dataBits; _port.StopBits = stopBits;
        _port.Handshake = Handshake.None; _port.ReadTimeout = 500; _port.WriteTimeout = 500;
    }

    public void Open()
    {
        if (string.IsNullOrWhiteSpace(_port.PortName)) throw new InvalidOperationException("PortName not set.");
        _port.DataReceived += OnData;
        _port.Open();
    }

    public void Close()
    {
        try { _port.DataReceived -= OnData; if (_port.IsOpen) _port.Close(); } catch { /* ignore */ }
    }

    public Task WriteAsync(byte[] bytes, CancellationToken ct) => Task.Run(() => _port.Write(bytes, 0, bytes.Length), ct);

    private void OnData(object? s, SerialDataReceivedEventArgs e)
    {
        try
        {
            var count = _port.BytesToRead;
            if (count <= 0) return;
            var buf = new byte[count];
            _port.Read(buf, 0, count);
            BytesReceived?.Invoke(this, buf);
        }
        catch { /* swallow */ }
    }

    public void Dispose() => Close();
}
