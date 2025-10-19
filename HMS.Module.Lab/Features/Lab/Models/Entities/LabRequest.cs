// Features/Lab/Models/Entities/myLabRequest.cs
using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Models.Entities;

public sealed class myLabRequest
{
    public long LabRequestId { get; set; }
    public long PatientId { get; set; }
    public long? AdmissionId { get; set; }
    public long? DoctorId { get; set; }
    public string Priority { get; set; } = "Routine";
    public LabRequestStatus Status { get; set; } = LabRequestStatus.Requested;
    public string? Notes { get; set; }
    public string OrderNo { get; set; } = default!;   // business number

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }

    public ICollection<myLabRequestItem> Items { get; set; } = new List<myLabRequestItem>();
}
