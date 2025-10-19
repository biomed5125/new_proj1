// Features/Lab/Models/Entities/myLabSample.cs
using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabSample
{
    public long LabSampleId { get; set; }
    public long LabRequestId { get; set; }
    public string AccessionNumber { get; set; } = default!;
    public LabSampleStatus Status { get; set; } = LabSampleStatus.Collected;

    public DateTime? CollectedAtUtc { get; set; }
    public string? CollectedBy { get; set; }
    public DateTime? ReceivedAtUtc { get; set; }
    public string? ReceivedBy { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
