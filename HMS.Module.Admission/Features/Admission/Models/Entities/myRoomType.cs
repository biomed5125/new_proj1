namespace HMS.Module.Admission.Features.Admission.Models.Entities;

public sealed class myRoomType
{
    public long RoomTypeId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal DailyRate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}
