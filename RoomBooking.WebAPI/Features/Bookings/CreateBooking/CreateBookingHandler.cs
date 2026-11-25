using ErrorOr;
using Mediator;
using RoomBooking.Data;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Rooms;
using RoomBooking.WebAPI.Features.Rooms;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Features.Bookings.Dtos;

namespace RoomBooking.WebAPI.Features.Bookings.CreateBooking;

public sealed class CreateBookingHandler(AppDbContext thisDbContext) : ICommandHandler<CreateBookingCommand, ErrorOr<BookingDto>>
{
    #region ICommandHandler<CreateBookingCommand, ErrorOr<BookingDto>>
    public async ValueTask<ErrorOr<BookingDto>> Handle(CreateBookingCommand command, CancellationToken cancellationToken)
    {
        RoomEntity? room = await thisDbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(r => r.Id == command.RoomId, cancellationToken);
        if(room is null)
        {
            return RoomErrors.RoomNotFound(command.RoomId);
        }

        BookingEntity booking = command.ToSource();
        booking.UserId = command.User.Id;
        booking.PaymentState = room.IsFree ? BookingPaymentState.NotRequired : BookingPaymentState.PendingPayment;
        DateTime now = DateTime.UtcNow;
        if(booking.PaymentState == BookingPaymentState.PendingPayment)
        {
            booking.PendingPaymentExpireAt = now.AddMinutes(15);
        }
        booking.StartTime = booking.StartTime.ToUniversalTime();
        booking.EndTime = booking.EndTime.ToUniversalTime();

        bool isOverlapping = await thisDbContext.Bookings.AsNoTracking().AnyAsync(
            b => b.RoomId == booking.RoomId
                 && !b.Cancelled
                 && (b.PaymentState == BookingPaymentState.NotRequired
                     || b.PaymentState == BookingPaymentState.PaymentSucceded
                     || (b.PaymentState == BookingPaymentState.PendingPayment && b.PendingPaymentExpireAt < now))
                 && booking.StartTime < b.EndTime
                 && booking.EndTime > b.StartTime,
            cancellationToken
        );
        if(isOverlapping)
        {
            return BookingErrors.BookingOverlaps();
        }

        await thisDbContext.AddAsync(booking, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);
        return BookingDto.FromSource(booking);
    }
    #endregion
}