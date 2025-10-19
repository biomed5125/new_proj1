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

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    // NEW: set by the labels page when opened/printed
    public bool LabelPrinted { get; set; }  // default false

    public myLabRequest Request { get; set; } = default!;
}
