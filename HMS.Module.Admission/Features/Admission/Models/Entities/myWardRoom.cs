namespace HMS.Module.Admission.Features.Admission.Models.Entities;

public sealed class myWardRoom
{
    public long WardRoomId { get; set; }
    public long WardId { get; set; }
    public long RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
