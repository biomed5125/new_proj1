using System.Text;

namespace HMS.CommBench.Core;

public sealed class AstmSession
{
    private readonly SerialPortService _port;
    private char _seq = '1';

    public AstmSession(SerialPortService port) => _port = port;

    public event EventHandler<string>? TraceOut;  // "[time] OUT ..."
    public event EventHandler<string>? TraceIn;   // "[time] IN  ..."

    private static byte[] B(params byte[] bs) => bs;

    private Task SendCtrl(byte b, CancellationToken ct)
        => _port.WriteAsync(B(b), ct);

    private Task SendFrame(string payload, CancellationToken ct)
    {
        var frame = AstmCodec.MakeFrame(_seq, payload);
        _seq = _seq == '7' ? '1' : (char)(_seq + 1);
        return _port.WriteAsync(frame, ct);
    }

    public async Task SendSequenceAsync(IEnumerable<string> payloads, CancellationToken ct)
    {
        await SendCtrl(AstmCodec.ENQ, ct); TraceOut?.Invoke(this, "ENQ");
        // wait small time for ACK (demo – proper impl would await real input signal)
        await Task.Delay(100, ct);

        foreach (var p in payloads)
        {
            await SendFrame(p, ct); TraceOut?.Invoke(this, $"FRAME {p}");
            await Task.Delay(100, ct); // wait for ACK in real impl
        }
        await SendCtrl(AstmCodec.EOT, ct); TraceOut?.Invoke(this, "EOT");
    }

    public void OnBytesReceived(byte[] bytes)
    {
        // naive parser: print control / try frames
        foreach (var b in bytes)
        {
            if (b is AstmCodec.ACK or AstmCodec.NAK or AstmCodec.ENQ or AstmCodec.EOT)
            {
                var name = b switch { AstmCodec.ACK => "ACK", AstmCodec.NAK => "NAK", AstmCodec.ENQ => "ENQ", AstmCodec.EOT => "EOT", _ => $"0x{b:X2}" };
                TraceIn?.Invoke(this, name);
            }
        }

        var span = bytes.AsSpan();
        while (span.Length > 0)
        {
            if (AstmCodec.TryParse(span, out var frameText, out var used))
            {
                TraceIn?.Invoke(this, "FRAME " + frameText);
                span = span.Slice(used);
            }
            else
            {
                // if TryParse set consumed>0, skip; otherwise break
                // (see codec; here we just break to avoid infinite loop)
                break;
            }
        }
    }
}
