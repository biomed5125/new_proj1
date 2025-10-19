namespace HMS.Module.Admission.Features.Admission.Models.Dtos;

public sealed record WardRoomDto(long WardRoomId, long WardId, long RoomTypeId, string RoomNumber, int Capacity);
