namespace HMS.Api.Features.Admission.Models.Dtos;

public class TransferAdmissionDto
{
    public long AdmissionId { get; set; }
    public long NewWardRoomId { get; set; }
    public DateTime TransferAtUtc { get; set; }
    public string? Notes { get; set; }
}
