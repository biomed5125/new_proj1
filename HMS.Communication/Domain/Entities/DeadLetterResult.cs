// HMS.Communication/Domain/Entities/DeadLetterResult.cs
using System;

namespace HMS.Communication.Domain.Entities;
public sealed class DeadLetterResult
{
    public long DeadLetterResultId { get; set; }
    public long DeviceId { get; set; }
    public string Reason { get; set; } = "";
    public string Payload { get; set; } = "";
    public DateTime At { get; set; }
}
