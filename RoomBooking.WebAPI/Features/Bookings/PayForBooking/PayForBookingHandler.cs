using ErrorOr;
using Mediator;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Features.Bookings;
using RoomBooking.WebAPI.Features.Payment.Pay;

namespace RoomBooking.WebAPI.Features.Bookings.PayForBooking;

public sealed class PayForBookingHandler(AppDbContext thisDbContext, IMediator thisMediator) : ICommandHandler<PayForBookingCommand, ErrorOr<Unit>>
{
    #region ICommandHandler<PayForBookingCommand, ErrorOr<Unit>>
    public async ValueTask<ErrorOr<Unit>> Handle(PayForBookingCommand command, CancellationToken cancellationToken)
    {
        BookingEntity? booking = await thisDbContext.Bookings.FirstOrDefaultAsync(b => b.Id == command.BookingId, cancellationToken);
        if(booking is null)
        {
            return BookingErrors.BookingNotFound(command.BookingId);
        }
        if(booking.UserId != command.User.Id)
        {
            return BookingErrors.BookingAccessDenied(command.BookingId);
        }
        if(booking.Cancelled
           || booking.PaymentState is not (BookingPaymentState.PendingPayment or BookingPaymentState.PaymentFailed))
        {
            return BookingErrors.BookingCannotBePaid(command.BookingId);
        }

        ErrorOr<Unit> paymentResult = await thisMediator.Send(new PayCommand(command.Price, command.Currency), cancellationToken);
        booking.PaymentState = paymentResult.IsError ? BookingPaymentState.PaymentFailed : BookingPaymentState.PaymentSucceded;
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return paymentResult;
    }
    #endregion
}
