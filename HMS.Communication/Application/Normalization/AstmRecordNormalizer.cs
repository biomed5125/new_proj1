// HMS.Communication/Application/Normalization/AstmRecordNormalizer.cs
using HMS.Communication.Abstractions;

namespace HMS.Communication.Application.Normalization
{
    public sealed class AstmRecordNormalizer : IRecordNormalizer
    {
        public IEnumerable<NormalizedEvent> Normalize(ParsedRecord rec)
        {
            if (!string.Equals(rec.Protocol, "ASTM", StringComparison.Ordinal)) yield break;

            if (rec.Kind == RecordKind.Result && !string.IsNullOrWhiteSpace(rec.Accession))
            {
                // stable event id (per result line)
                var evtId = $"ASTM:{rec.Device.Id}:{rec.Accession}:{rec.InstrumentCode}:{rec.At.UtcDateTime:yyyyMMddHHmmssfff}";

                yield return new NormalizedEvent(
                    Device: rec.Device,
                    At: rec.At,
                    Kind: EventKind.ResultPosted,
                    Accession: rec.Accession,
                    LabTestCode: null,              // set by mapper/router later if needed
                    InstrumentCode: rec.InstrumentCode,
                    Value: rec.Value,
                    Units: rec.Units,
                    Flag: rec.Flag,
                    Notes: null,
                    EventId: evtId
                );
            }
            else if (rec.Kind is RecordKind.Header or RecordKind.Patient or RecordKind.Order or RecordKind.Terminator)
            {
                // You could emit lightweight “trace” events here if you ever want,
                // but functionally we only route ResultPosted.
                yield break;
            }
        }
    }
}