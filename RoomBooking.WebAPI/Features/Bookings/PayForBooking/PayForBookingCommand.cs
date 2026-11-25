using ErrorOr;
using Mediator;
using RoomBooking.WebAPI.Behaviors.Authorized;

namespace RoomBooking.WebAPI.Features.Bookings.PayForBooking;

public sealed record class PayForBookingCommand(Guid BookingId, decimal Price, string Currency) : ICommand<ErrorOr<Unit>>, IAuthorizedBehaviorMessage;
