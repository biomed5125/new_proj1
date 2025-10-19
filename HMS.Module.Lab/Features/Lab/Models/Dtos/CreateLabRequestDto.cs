// Features/Lab/Models/Dtos/LabRequestDtos.cs
using HMS.Module.Lab.Features.Lab.Models.Enums;

namespace HMS.Module.Lab.Features.Lab.Models.Dtos;

public sealed class CreateLabRequestDto
{
    public long PatientId { get; set; }
    public long? AdmissionId { get; set; }
    public long? DoctorId { get; set; }
    public string Priority { get; set; } = "Routine";
    public string? Notes { get; set; }
    public List<long> TestIds { get; set; } = new();
}
