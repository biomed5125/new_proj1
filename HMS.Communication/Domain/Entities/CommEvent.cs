// HMS.Communication/Domain/Entities/CommEvent.cs
using System;

namespace HMS.Communication.Domain.Entities;
public sealed class CommEvent
{
    public long CommEventId { get; set; }
    public string EventId { get; set; } = default!;
    public long DeviceId { get; set; }
    public string Kind { get; set; } = default!;
    public string? Accession { get; set; }
    public string? LabTestCode { get; set; }
    public string? InstrumentCode { get; set; }
    public string? Value { get; set; }
    public string? Units { get; set; }
    public string? Flag { get; set; }
    public DateTime At { get; set; }
}
