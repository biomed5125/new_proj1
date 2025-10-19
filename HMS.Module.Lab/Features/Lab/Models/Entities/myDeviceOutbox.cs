namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myDeviceOutbox
{
    public long DeviceOutboxId { get; set; }
    public string DeviceName { get; set; } = default!;
    public DateTime QueuedAtUtc { get; set; }
    public string Payload { get; set; } = default!;
    public bool Sent { get; set; }
    public bool IsDeleted { get; set; }
}
