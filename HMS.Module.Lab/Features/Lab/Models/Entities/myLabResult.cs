using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabResult
{
    public long LabResultId { get; set; }

    public long LabRequestId { get; set; }
    public long LabRequestItemId { get; set; }

    // Informational audit (nullable for backfill)
    public long? DeviceId { get; set; }
    public string? AccessionNumber { get; set; }
    public string? InstrumentTestCode { get; set; }  // raw from device  

    // Clinical values
    public string LabTestCode { get; set; } = default!;        // mirror item.Code
    public string? LabTestName { get; set; }                   // mirror item.Name
    public string? Value { get; set; }
    public string? Unit { get; set; }
    public decimal? RefLow { get; set; }
    public decimal? RefHigh { get; set; }

    public ResultFlag? Flag { get; set; }
    public LabResultStatus Status { get; set; } = LabResultStatus.Entered;
    public string? RawFlag { get; set; }  // original flag text

    public long? ApprovedByDoctorId { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    // NEW: relational link (nullable FIRST; we’ll make it required after backfill)
    public long LabTestId { get; set; }
    public myLabTest? LabTest { get; set; }   // nav can stay nullable; FK drives the requiredness
}
