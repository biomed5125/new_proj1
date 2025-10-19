namespace HMS.Communication.Abstractions;

public readonly record struct DeviceRef(long Id, string Code);

public enum FrameDirection { Rx, Tx }

public sealed record RawFrame(
    DeviceRef Device,
    DateTimeOffset At,
    FrameDirection Dir,
    string Transport,
    byte[] Bytes,
    string Ascii
);

public enum RecordKind { Header, Patient, Order, Result, Terminator, Unknown }

public sealed record ParsedRecord(
    DeviceRef Device,
    DateTimeOffset At,
    string Protocol,
    RecordKind Kind,
    string RawText,
    string? Accession,
    string? InstrumentCode,
    string? Value,
    string? Units,
    string? Flag
);

public enum EventKind { ResultPosted, OrderDownloaded, MappingMissing, DecodeFail, Ack, Nak, StatusChanged }

public sealed record NormalizedEvent(
    DeviceRef Device,
    DateTimeOffset At,
    EventKind Kind,
    string? Accession,
    string? LabTestCode,
    string? InstrumentCode,
    string? Value,
    string? Units,
    string? Flag,
    string? Notes,
    string EventId
);
public sealed record OrderLine(string LisCode, string InstrumentCode, string? Name);
public sealed record OrderDownload(string Accession, string Priority, IReadOnlyList<OrderLine> Lines);
public sealed record AstmOptions(
    string Mode,
    bool TsInquireAlways,
    string IdKind,
    int ReplyWindowMs,
    int MaxRetries,
    string Delimiters,
    int Baud, string Parity, int DataBits, int StopBits,
    string Port,
    string? Host = null, int? PortTcp = null
);