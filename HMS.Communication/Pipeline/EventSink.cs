// HMS.Communication/Pipeline/EventSink.cs
using HMS.Communication.Abstractions;
using HMS.Communication.Domain.Entities;
using HMS.Communication.Persistence;

namespace HMS.Communication.Pipeline;

public sealed class EventSink : IEventSink
{
    private readonly CommunicationDbContext _db;
    public EventSink(CommunicationDbContext db) => _db = db;

    public async Task PersistsAsync(RawFrame frame, ParsedRecord? parsed, NormalizedEvent? ev, CancellationToken ct)
    {
        // Write the raw inbound ONCE: only when this is the "raw" call.
        // (parsed == null && ev == null) is the first call the adapter makes per chunk.
        if (parsed is null && ev is null)
        {
            _db.Inbound.Add(new CommMessageInbound
            {
                DeviceId = frame.Device.Id,
                At = frame.At.UtcDateTime,
                Direction = frame.Dir.ToString(),   // "Rx" / "Tx"
                Transport = frame.Transport,        // "FileFeed" / "Serial" / "Tcp"
                Ascii = frame.Ascii,
                Bytes = frame.Bytes
            });
        }

        // If there is a normalized event, persist it (and make sure EventId is never null)
        if (ev is not null)
        {
            // Fallback EventId if a producer left it blank
            var eventId = string.IsNullOrWhiteSpace(ev.EventId)
                ? $"EV:{ev.Device.Id}:{DateTime.UtcNow:yyyyMMddHHmmssfff}:{Guid.NewGuid():N}"
                : ev.EventId;

            _db.Events.Add(new CommEvent
            {
                EventId = eventId,
                DeviceId = ev.Device.Id,
                Kind = ev.Kind.ToString(),
                Accession = ev.Accession,
                LabTestCode = ev.LabTestCode,
                InstrumentCode = ev.InstrumentCode,
                Value = ev.Value,
                Units = ev.Units,
                Flag = ev.Flag,
                At = ev.At.UtcDateTime
            });
        }

        await _db.SaveChangesAsync(ct);
    }

    public Task PublishAsync(NormalizedEvent ev, CancellationToken ct) => Task.CompletedTask;
}
