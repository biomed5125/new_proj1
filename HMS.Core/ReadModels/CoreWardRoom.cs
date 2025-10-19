// CoreWardRoom.cs
namespace HMS.Core.ReadModels;
public sealed class CoreWardRoom
{
    public long WardRoomId { get; set; }
    public long WardId { get; set; }
    public long RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = "";
    public int Capacity { get; set; }
}