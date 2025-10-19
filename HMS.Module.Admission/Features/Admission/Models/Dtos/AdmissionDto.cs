namespace HMS.Module.Admission.Features.Admission.Models.Dtos;

public sealed record AdmissionDto(
    long AdmissionId,
    long PatientId,
    long? DoctorId,
    long? WardRoomId,
    string EncounterNo,
    DateTime AdmittedAtUtc,
    DateTime? DischargedAtUtc,
    int Status,
    string? DiagnosisOnAdmission,
    string? Notes
);
