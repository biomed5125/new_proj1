using HMS.Communication.Abstractions;
using HMS.Communication.Application.Protocols.ASTM;
using System.Buffers;
using System.Text;

/// <summary>
/// ASTM adapter — used for both live serial/TCP and file-feed transports.
/// Handles ASTM handshakes, parses results, and pushes events.
/// </summary>
public sealed class AstmAdapter : IProtocolAdapter
{
    private readonly AstmParser _parser = new();

    public string Protocol => "ASTM";

    public async Task RunAsync(
        IChannel channel,
        IRecordNormalizer normalizer,
        IMessageRouter msgRouter,
        IEventSink sink,
        IFrameTracer tracer,
        CancellationToken ct)
    {
        var dev = channel.Device;
        var buffer = ArrayPool<byte>.Shared.Rent(4096);
        bool isFile = string.Equals(channel.Transport.Name, "FileFeed", StringComparison.OrdinalIgnoreCase);

        try
        {
            while (!ct.IsCancellationRequested)
            {
                var n = await channel.ReadAsync(buffer.AsMemory(0, buffer.Length), ct);

                // For file feeds, EOF (n==0) means we're done — let host archive
                if (n <= 0)
                {
                    if (isFile) break;
                    await Task.Delay(50, ct);
                    continue;
                }

                var ascii = Encoding.ASCII.GetString(buffer, 0, n);
                var rx = new RawFrame(dev, DateTimeOffset.UtcNow, FrameDirection.Rx, channel.Transport.Name, buffer[..n].ToArray(), ascii);

                await tracer.TraceAsync(rx, ct);
                await sink.PersistsAsync(rx, parsed: null, ev: null, ct);

                // ASTM control handshake
                if (ascii.IndexOf((char)AstmConstants.ENQ) >= 0)
                    await channel.WriteAsync(new byte[] { AstmConstants.ACK }, ct);
                if (ascii.IndexOf((char)AstmConstants.EOT) >= 0)
                {
                    if (isFile) break;
                    continue;
                }

                foreach (var rec in _parser.Parse(dev, ascii))
                {
                    foreach (var ev in normalizer.Normalize(rec))
                    {
                        await sink.PersistsAsync(rx, rec, ev, ct);
                        await sink.PublishAsync(ev, ct);
                        await msgRouter.RouteAsync(ev, ct);
                    }
                }

                if (ascii.Contains("\rO|") || ascii.Contains("\rR|") || ascii.StartsWith("O|") || ascii.StartsWith("R|"))
                    await channel.WriteAsync(new byte[] { AstmConstants.ACK }, ct);

                if (isFile) break;
            }
        }
        catch (OperationCanceledException)
        {
            // graceful shutdown
        }
        catch (Exception ex)
        {
            var ev = new NormalizedEvent(dev, DateTimeOffset.UtcNow, EventKind.DecodeFail,
                Accession: null, LabTestCode: null, InstrumentCode: null,
                Value: null, Units: null, Flag: null, Notes: ex.Message,
                EventId: $"ERR:{dev.Id}:{DateTime.UtcNow:yyyyMMddHHmmssfff}");
            await sink.PublishAsync(ev, ct);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }

    public Task<ParsedRecord?> ReadAsync(CancellationToken ct) => Task.FromResult<ParsedRecord?>(null);
    public Task SendOrderAsync(OrderDownload order, CancellationToken ct) => Task.CompletedTask;
}
