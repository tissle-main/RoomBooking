using ErrorOr;
using Mediator;

namespace RoomBooking.WebAPI.Features.Bookings.CancelBooking;

public sealed record class CancelBookingCommand(Guid BookingId) : ICommand<ErrorOr<Unit>>;