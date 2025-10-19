// HMS.Communication/Domain/Entities/CommMessageInbound.cs
using System;

namespace HMS.Communication.Domain.Entities;
public sealed class CommMessageInbound
{
    public long CommMessageInboundId { get; set; }
    public long DeviceId { get; set; }
    public DateTime At { get; set; }
    public string Direction { get; set; } = "Rx";
    public string Transport { get; set; } = "";
    public string Ascii { get; set; } = "";
    public byte[] Bytes { get; set; } = Array.Empty<byte>();
    public string? BusinessNo { get; set; }          // <—
}
