using ErrorOr;

namespace RoomBooking.WebAPI.Features.Rooms;

public static class RoomErrors
{
    public const string RoomNotFoundErrorCode = "Room.NotFound";

    public static Error RoomNotFound(Guid roomId)
    {
        return Error.NotFound(RoomNotFoundErrorCode, $"Room with id '{roomId}' not found.");
    }
}