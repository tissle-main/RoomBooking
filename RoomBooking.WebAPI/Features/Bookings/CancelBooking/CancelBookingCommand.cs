using ErrorOr;
using Mediator;
using RoomBooking.WebAPI.Behaviors.Authorized;

namespace RoomBooking.WebAPI.Features.Bookings.CancelBooking;

public sealed record class CancelBookingCommand(Guid BookingId) : ICommand<ErrorOr<Unit>>, IAuthorizedBehaviorMessage;