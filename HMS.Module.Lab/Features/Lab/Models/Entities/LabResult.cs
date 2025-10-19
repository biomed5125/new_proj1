// Features/Lab/Models/Entities/myLabResult.cs
using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabResult
{
    public long LabResultId { get; set; }
    public long LabRequestId { get; set; }
    public long LabRequestItemId { get; set; }

    public string? Value { get; set; }
    public string? Unit { get; set; }
    public decimal? RefLow { get; set; }
    public decimal? RefHigh { get; set; }
    public string? Flag { get; set; }
    public LabResultStatus Status { get; set; } = LabResultStatus.Entered;

    public long? ApprovedByDoctorId { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
