namespace HMS.Module.Admission.Features.Admission.Models.Dtos;

public sealed class DischargeAdmissionDto
{
    public DateTime DischargedAtUtc { get; set; }
    public string? Notes { get; set; }
}
