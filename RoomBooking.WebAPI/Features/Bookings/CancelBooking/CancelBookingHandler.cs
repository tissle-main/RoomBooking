using ErrorOr;
using Mediator;
using RoomBooking.Data;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Features.Bookings;

namespace RoomBooking.WebAPI.Features.Bookings.CancelBooking;

public sealed class CancelBookingHandler(AppDbContext thisDbContext) : ICommandHandler<CancelBookingCommand, ErrorOr<Unit>>
{
    #region ICommandHandler<CancelBookingCommand, ErrorOr<Unit>>
    public async ValueTask<ErrorOr<Unit>> Handle(CancelBookingCommand command, CancellationToken cancellationToken)
    {
        Guid userId = command.User.Id;
        BookingEntity? booking = await thisDbContext.Bookings.FirstOrDefaultAsync(b => b.Id == command.BookingId, cancellationToken);
        if(booking is null)
        {
            return BookingErrors.BookingNotFound(command.BookingId);
        }
        if(booking.UserId != userId)
        {
            return BookingErrors.BookingAccessDenied(command.BookingId);
        }

        booking.Cancelled = true;
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
    #endregion
}