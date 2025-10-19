// CoreRoomType.cs
namespace HMS.Core.ReadModels;
public sealed class CoreRoomType
{
    public long RoomTypeId { get; set; }
    public string Name { get; set; } = "";
    public decimal DailyRate { get; set; }
}