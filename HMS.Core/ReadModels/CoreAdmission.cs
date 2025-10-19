// CoreAdmission.cs
namespace HMS.Core.ReadModels;
public sealed class CoreAdmission
{
    public long AdmissionId { get; set; }
    public long PatientId { get; set; }
    public long? DoctorId { get; set; }
    public long WardRoomId { get; set; }
    public string? EncounterNo { get; set; }
    public DateTime AdmittedAtUtc { get; set; }
    public DateTime? DischargedAtUtc { get; set; }
    public int Status { get; set; }                // mirror enum int
    public string? DiagnosisOnAdmission { get; set; }
    public string? Notes { get; set; }
}