namespace HMS.Module.Admission.Features.Admission.Models.Dtos;

public sealed class CreateAdmissionDto
{
    public long PatientId { get; set; }
    public long? DoctorId { get; set; }
    public long? WardRoomId { get; set; }
    public DateTime AdmittedAtUtc { get; set; }
    public string? DiagnosisOnAdmission { get; set; }
    public string? Notes { get; set; }
}
