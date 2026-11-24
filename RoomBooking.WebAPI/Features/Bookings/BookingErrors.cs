using ErrorOr;

namespace RoomBooking.WebAPI.Features.Bookings;

public static class BookingErrors
{
    public const string BookingNotFoundErrorCode = "Booking.NotFound";
    public const string BookingOverlapsErrorCode = "Booking.Overlaps";

    public static Error BookingNotFound(Guid bookingId)
    {
        return Error.NotFound(BookingNotFoundErrorCode, $"Booking with id '{bookingId}' not found.");
    }
    public static Error BookingOverlaps()
    {
        return Error.Conflict(BookingOverlapsErrorCode, "The room is already booked during the requested time period.");
    }
}