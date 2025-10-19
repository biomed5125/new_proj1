using HMS.Module.Admission.Features.Admission.Models.Enums;

namespace HMS.Module.Admission.Features.Admission.Models.Entities;

public sealed class myAdmission
{
    public long AdmissionId { get; set; }
    public long PatientId { get; set; }
    public long? DoctorId { get; set; }
    public long? WardRoomId { get; set; }

    public string EncounterNo { get; set; } = string.Empty;

    public DateTime AdmittedAtUtc { get; set; }
    public DateTime? DischargedAtUtc { get; set; }

    public AdmissionStatus Status { get; set; }

    public string? DiagnosisOnAdmission { get; set; }
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
