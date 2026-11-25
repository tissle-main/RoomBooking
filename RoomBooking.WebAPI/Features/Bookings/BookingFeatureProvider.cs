using RoomBooking.WebAPI.Features.Bookings.CancelBooking;
using RoomBooking.WebAPI.Features.Bookings.CreateBooking;
using RoomBooking.WebAPI.Features.Bookings.GetActiveBookings;
using RoomBooking.WebAPI.Features.Bookings.PayForBooking;

namespace RoomBooking.WebAPI.Features.Bookings;

public sealed class BookingFeatureProvider : FeatureProvider
{
    #region Base
    public override void MapEndpoints(WebApplication app)
    {
        app.MapGetActiveBookingsEndpoint();
        app.MapCreateBookingEndpoint();
        app.MapCancelBookingEndpoint();
        app.MapPayForBookingEndpoint();
    }
    #endregion
}