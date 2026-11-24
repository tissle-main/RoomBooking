using RoomBooking.WebAPI.Features.Bookings.CancelBooking;
using RoomBooking.WebAPI.Features.Bookings.CreateBooking;
using RoomBooking.WebAPI.Features.Bookings.GetActiveBookings;

namespace RoomBooking.WebAPI.Features.Bookings;

public sealed class BookingFeatureProvider : FeatureProvider
{
    #region Base
    public override void MapEndpoints(WebApplication app)
    {
        app.MapGetActiveBookingsEndpoint();
        app.MapCreateBookingEndpoint();
        app.MapCancelBookingEndpoint();
    }
    #endregion
}