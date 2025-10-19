// HMS.Communication/Domain/Entities/CommMessageOutbound.cs
using System;

namespace HMS.Communication.Domain.Entities;
public sealed class CommMessageOutbound
{
    public long CommMessageOutboundId { get; set; }
    public long DeviceId { get; set; }
    public DateTime At { get; set; }
    public string Transport { get; set; } = "";
    public string Payload { get; set; } = "";
    public bool Sent { get; set; }
    public string? BusinessNo { get; set; }          // <—
}
