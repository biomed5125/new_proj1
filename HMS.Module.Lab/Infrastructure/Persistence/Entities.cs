// HMS.Communication/Infrastructure/Persistence/Entities.cs
namespace HMS.Communication.Infrastructure.Persistence.Entities;

public enum MessageDirection { Outbound = 0, Inbound = 1 }
public enum MessageStatus { Queued = 0, Sending = 1, Sent = 2, Received = 3, Parsed = 4, Failed = 9 }

public sealed class CommDevice
{
    public long CommDeviceId { get; set; }
    public string Name { get; set; } = "";
    public string Model { get; set; } = "";      // e.g., "Cobas c311"
    public string Protocol { get; set; } = "ASTM";
    public string Transport { get; set; } = "Serial"; // Serial | Tcp
    public string TransportSettingsJson { get; set; } = "{}"; // port/baud or host/port
    public bool SendOrders { get; set; } = true;
    public bool ReceiveResults { get; set; } = true;
    public bool IsEnabled { get; set; } = true;

    public DateTime CreatedAt { get; set; }
}

public sealed class CommMessage
{
    public long CommMessageId { get; set; }
    public long CommDeviceId { get; set; }
    public MessageDirection Direction { get; set; }
    public MessageStatus Status { get; set; }
    public string Payload { get; set; } = "";          // raw ASTM text (multiple frames concatenated)
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public sealed class LabTestInstrumentMap
{
    public long LabTestInstrumentMapId { get; set; }
    public long CommDeviceId { get; set; }
    public long LabTestId { get; set; }          // from HMS.Module.Lab
    public string InstrumentTestCode { get; set; } = ""; // code used on analyzer
}
