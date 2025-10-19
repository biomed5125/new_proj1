namespace HMS.Module.Admission.Features.Admission.Models.Dtos;

public sealed class UpdateAdmissionDto
{
    public long? DoctorId { get; set; }
    public long? WardRoomId { get; set; }
    public string? DiagnosisOnAdmission { get; set; }
    public string? Notes { get; set; }
    public int? Status { get; set; }   // optional transition (e.g., Cancelled)
}
