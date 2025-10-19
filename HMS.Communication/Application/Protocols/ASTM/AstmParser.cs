using HMS.Communication.Abstractions;
using System;

namespace HMS.Communication.Application.Protocols.ASTM
{
    /// <summary>
    /// ASTM frame parser for Cobas / Sysmex / Fuji style text files.
    /// Extracts clean record lines and carries accession numbers forward.
    /// </summary>
    public sealed class AstmParser
    {
        private string? _currentAccession;

        public IEnumerable<ParsedRecord> Parse(DeviceRef dev, string asciiDump)
        {
            if (string.IsNullOrWhiteSpace(asciiDump))
                yield break;

            var lines = asciiDump.Split('\r', StringSplitOptions.RemoveEmptyEntries);

            foreach (var raw in lines)
            {
                var clean = CleanLine(raw);
                if (string.IsNullOrWhiteSpace(clean))
                    continue;

                var kind = clean.Length > 0 ? MapKind(clean[0]) : RecordKind.Unknown;
                var fields = clean.Split('|');

                if (clean.StartsWith("O|", StringComparison.Ordinal))
                {
                    // O|seq|O.3|O.4|...
                    var o3 = fields.Length > 2 ? NullIfEmpty(fields[2]) : null;
                    var o4 = fields.Length > 3 ? NullIfEmpty(fields[3]) : null;
                    _currentAccession = CanonicalAccession(o3, o4);

                    yield return new ParsedRecord(
                        dev, DateTimeOffset.UtcNow, "ASTM", RecordKind.Order,
                        clean, _currentAccession, null, null, null, null);
                }
                else if (clean.StartsWith("R|", StringComparison.Ordinal))
                {
                    // R|seq|^^^GLU$SC|155|mg/dL|70-110|N|F|YYYYMMDDhhmmss
                    string? instrCode = null, value = null, units = null, flag = null;

                    if (fields.Length > 2)
                        instrCode = ExtractInstrumentCode(fields[2]);

                    if (fields.Length > 3) value = NullIfEmpty(fields[3]);
                    if (fields.Length > 4) units = NullIfEmpty(fields[4]);

                    if (fields.Length > 6) flag = NullIfEmpty(fields[6]);
                    else if (fields.Length > 5) flag = NullIfEmpty(fields[5]);

                    yield return new ParsedRecord(
                        dev, DateTimeOffset.UtcNow, "ASTM", RecordKind.Result,
                        clean, _currentAccession, instrCode, value, units, flag);
                }
                else
                {
                    yield return new ParsedRecord(
                        dev, DateTimeOffset.UtcNow, "ASTM", kind,
                        clean, _currentAccession, null, null, null, null);

                    if (kind == RecordKind.Terminator)
                        _currentAccession = null;
                }
            }
        }

        private static RecordKind MapKind(char tag) => tag switch
        {
            'H' => RecordKind.Header,
            'P' => RecordKind.Patient,
            'O' => RecordKind.Order,
            'R' => RecordKind.Result,
            'L' => RecordKind.Terminator,
            _ => RecordKind.Unknown
        };

        private static string? NullIfEmpty(string? s)
            => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        private static string? CanonicalAccession(string? o3, string? o4)
        {
            if (!string.IsNullOrWhiteSpace(o3)) return o3.Trim();
            if (!string.IsNullOrWhiteSpace(o4) && !o4.Trim().Equals("N", StringComparison.OrdinalIgnoreCase))
                return o4.Trim();
            return null;
        }

        private static string? ExtractInstrumentCode(string? field)
        {
            if (string.IsNullOrWhiteSpace(field)) return null;
            var comp = field.Split('^');
            var code = comp.Length >= 4 ? comp[3] : comp[^1];
            if (string.IsNullOrWhiteSpace(code)) return null;

            var trimmed = code.Trim();
            var dollar = trimmed.IndexOf('$');
            return dollar >= 0 ? trimmed[..dollar] : trimmed;
        }

        private static string CleanLine(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            var span = s.AsSpan();

            if (span.Length > 0 && span[^1] == '\n')
                span = span[..^1];

            while (span.Length > 0 && (span[0] == '\x02' || span[0] == '\x05'))
                span = span[1..];
            while (span.Length > 0 && (span[^1] == '\x03' || span[^1] == '\x04'))
                span = span[..^1];

            return span.ToString();
        }
    }
}
