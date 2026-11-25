using ErrorOr;
using Mediator;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Features.Bookings.Dtos;

namespace RoomBooking.WebAPI.Features.Bookings.GetActiveBookings;

public sealed record class GetActiveBookingsQuery(
    Guid RoomId,
    DateOnly Day
) : IQuery<ErrorOr<IEnumerable<BookingDto>>>, IAuthorizedBehaviorMessage;