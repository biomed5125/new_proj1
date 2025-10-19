using HMS.Module.Lab.Features.Lab.Models.Enums;
namespace HMS.Module.Lab.Features.Lab.Models.Entities;
public sealed class myLabRequest
{
    public long LabRequestId { get; set; }
    public long? AdmissionId { get; set; }

    public long? PatientId { get; set; }    
    public long? DoctorId { get; set; }
    public long? LabPatientId { get; set; }   // local patient (LIS mode)
    public long? LabDoctorId { get; set; }   // local doctor (LIS mode)

    // denormalized helpers (optional)
    // myLabRequest (add these nullable props)
    public string? PatientDisplay { get; set; }     // "Ahmed Ali / M / 1989-04-10"
    public string? DoctorDisplay { get; set; }     // "Dr. Sara Mostafa"
    public DateTime? PaidAtUtc { get; set; }     // null = not paid
    public string? Source { get; set; }     // "WalkIn", "HIS", etc.

    public myLabPatient? LabPatient { get; set; }
    public myLabDoctor? LabDoctor { get; set; }

    public string Priority { get; set; } = "Routine";
    public LabRequestStatus Status { get; set; } = LabRequestStatus.Requested;
    public string? Notes { get; set; }
    public string OrderNo { get; set; } = default!;    // business number

    // audit
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<myLabSample> Samples { get; set; } = new List<myLabSample>();
    public ICollection<myLabRequestItem> Items { get; set; } = new List<myLabRequestItem>();
}
