using HMS.Communication.Domain.Entities;

namespace HMS.Communication.Infrastructure.Persistence.Entities;
public sealed class CommDevice
{
    public long CommDeviceId { get; set; }   // ← Device ID
    public string Name { get; set; } = default!;
    public string DeviceCode { get; set; } = default!;   // ← Human/stable code (e.g., COBAS-E411-1)
    public string PortName { get; set; } = default!;   // ← COM5 / /dev/ttyS3 / tcp://...
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public long AnalyzerProfileId { get; set; }             // ← links to profile (Roche, Sysmex, Fuji)
    public AnalyzerProfile AnalyzerProfile { get; set; } = default!;
}

