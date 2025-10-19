namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myDeviceInbox
{
    public long DeviceInboxId { get; set; }
    public string DeviceName { get; set; } = default!;
    public DateTime ReceivedAtUtc { get; set; }
    public string Payload { get; set; } = default!;
    public bool Processed { get; set; }

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
