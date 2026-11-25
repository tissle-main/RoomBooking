using ErrorOr;
using Mediator;
using RoomBooking.Data;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.Data.Entities.Rooms;
using RoomBooking.WebAPI.Features.Rooms;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Features.Bookings.Dtos;

namespace RoomBooking.WebAPI.Features.Bookings.GetActiveBookings;

public sealed class GetActiveBookingsHandler(
    AppDbContext thisDbContext
) : IQueryHandler<GetActiveBookingsQuery, ErrorOr<IEnumerable<BookingDto>>>
{
    #region IQueryHandler<GetActiveBookingsQuery, ErrorOr<IEnumerable<BookingDto>>>
    public async ValueTask<ErrorOr<IEnumerable<BookingDto>>> Handle(GetActiveBookingsQuery query, CancellationToken cancellationToken)
    {
        RoomEntity? room = await thisDbContext.Rooms.AsNoTracking().FirstOrDefaultAsync(
            r => r.Id == query.RoomId,
            cancellationToken
        );
        if(room is null)
        {
            return RoomErrors.RoomNotFound(query.RoomId);
        }

        DateTime dayStart = query.Day.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        DateTime dayEnd = query.Day.AddDays(1).ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
        DateTime now = DateTime.UtcNow;
        Guid userId = query.User.Id;
        return await thisDbContext.Bookings.AsNoTracking().Where(
            b => b.RoomId == room.Id
                 && b.UserId == userId
                 && !b.Cancelled
                 && (b.PaymentState == BookingPaymentState.NotRequired
                     || b.PaymentState == BookingPaymentState.PaymentSucceded
                     || (b.PaymentState == BookingPaymentState.PendingPayment && b.PendingPaymentExpireAt < now))
                 && b.StartTime < dayEnd
                 && b.EndTime > dayStart
        ).OrderBy(b => b.StartTime).Select(BookingDto.Projection).ToArrayAsync(cancellationToken);
    }
    #endregion
}