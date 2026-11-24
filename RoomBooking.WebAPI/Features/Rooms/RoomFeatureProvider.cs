using RoomBooking.WebAPI.Features.Rooms.GetRooms;

namespace RoomBooking.WebAPI.Features.Rooms;

public sealed class RoomFeatureProvider : FeatureProvider
{
    #region Base
    public override void MapEndpoints(WebApplication app)
    {
        app.MapGetRoomsEndpoint();
    }
    #endregion
}