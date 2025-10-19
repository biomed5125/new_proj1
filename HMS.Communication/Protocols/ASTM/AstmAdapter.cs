// File: HMS.Communication/Application/Protocols/ASTM/AstmAdapter.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using HMS.Communication.Abstractions;

namespace HMS.Communication.Application.Protocols.ASTM
{
    /// <summary>
    /// ASTM adapter that reads one CR-terminated record per call via IChannel.
    /// </summary>
    public sealed class AstmAdapter : IProtocolAdapter
    {
        public string Protocol => "ASTM";

        private readonly IChannel _channel;
        private readonly DeviceRef _device;

        public AstmAdapter(IChannel channel, DeviceRef device)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _device = device;
        }

        public async Task<ParsedRecord?> ReadAsync(CancellationToken ct)
        {
            var line = await _channel.ReadAsync(ct);           // one ASTM record (no CR/LF)
            if (line is null) return null;
            return ParseLine(_device, line);
        }

        public async Task SendOrderAsync(OrderDownload order, CancellationToken ct)
        {
            // Minimal H/P/O*/L builder (text only)
            var text = AstmOutboundBuilder.Build(order);

            // Send each record as a line; channel adds CR if needed
            var lines = text.Split('\r', StringSplitOptions.RemoveEmptyEntries);
            foreach (var ln in lines)
                await _channel.WriteAsync(ln, ct);
        }

        // ---- helpers ----
        private static ParsedRecord? ParseLine(DeviceRef dev, string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return null;

            var clean = line.Trim('\r', '\n');
            var kind = clean.Length > 0 ? clean[0] switch
            {
                'H' => RecordKind.Header,
                'P' => RecordKind.Patient,
                'O' => RecordKind.Order,
                'R' => RecordKind.Result,
                'L' => RecordKind.Terminator,
                _ => RecordKind.Unknown
            } : RecordKind.Unknown;

            if (kind == RecordKind.Unknown) return null;

            string? accession = null, instr = null, value = null, units = null, flag = null;
            var f = clean.Split('|');

            if (kind == RecordKind.Order)
            {
                // O|1|<Accession>|...
                if (f.Length > 2) accession = NullIfEmpty(f[2]);
            }
            else if (kind == RecordKind.Result)
            {
                // R|1|^^^GLU^1|104|mg/dL|H|...
                if (f.Length > 2)
                {
                    var comp = f[2].Split('^');
                    instr = comp.Length >= 4 ? NullIfEmpty(comp[3]) : NullIfEmpty(comp[^1]);
                }
                if (f.Length > 3) value = NullIfEmpty(f[3]);
                if (f.Length > 4) units = NullIfEmpty(f[4]);
                if (f.Length > 5) flag = NullIfEmpty(f[5]);
            }

            return new ParsedRecord(
                dev,
                DateTimeOffset.UtcNow,
                "ASTM",
                kind,
                clean,
                accession,
                instr,
                value,
                units,
                flag
            );
        }

        private static string? NullIfEmpty(string s) => string.IsNullOrWhiteSpace(s) ? null : s;
    }

    internal static class AstmOutboundBuilder
    {
        public static string Build(OrderDownload order)
        {
            // Very small H/P/O*/L set for order download
            // H and P are minimal; extend to your analyzer’s spec as needed.
            var sb = new System.Text.StringBuilder();
            sb.Append("H|\\^&|||HOST|||||ASTM||P|1\r");
            sb.Append("P|1\r");
            int seq = 1;
            foreach (var l in order.Lines)
            {
                // O|<seq>|<Accession>|||<instr>^<lis>|R|<priority>
                sb.Append($"O|{seq}|{order.Accession}|||{l.InstrumentCode}^{l.LisCode}||R|{order.Priority}\r");
                seq++;
            }
            sb.Append("L|1|N\r");
            return sb.ToString();
        }
    }
}
