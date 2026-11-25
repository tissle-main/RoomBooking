using Bogus;
using RoomBooking.Data.Entities.Bookings;

namespace RoomBooking.WebAPI.Features.Bookings.Dtos;

public static class BookingEntityFaker
{
    private static Faker Faker { get; } = new();

    extension(Faker<BookingEntity> thisFaker)
    {
        public Faker<BookingEntity> Valid(Guid roomId = default, DateTimeOffset? minStartTime = null)
        {
            return thisFaker.CustomInstantiator(gen => new BookingEntity()
            {
                Title = gen.Name.FullName(),
                RoomId = roomId
            }).WithBookingPeriod(minStartTime ?? DateTimeOffset.UtcNow);
        }
        public Faker<BookingEntity> WithUserId(Guid userId)
        {
            return thisFaker.RuleFor(b => b.UserId, userId);
        }
        public Faker<BookingEntity> WithBookingPeriod(DateTimeOffset minStartTime)
        {
            DateTimeOffset startTime = Faker.Date.SoonOffset(refDate: minStartTime);
            DateTimeOffset endTime = Faker.Date.SoonOffset(refDate: startTime);
            thisFaker.RuleFor(b => b.StartTime, startTime);
            return thisFaker.RuleFor(b => b.EndTime, endTime);
        }
        public Faker<BookingEntity> WithPaymentState(BookingPaymentState paymentState)
        {
            return thisFaker.RuleFor(b => b.PaymentState, paymentState);
        }
        public Faker<BookingEntity> WithPendingPaymentExpiresAt(DateTime expiresAt)
        {
            return thisFaker.RuleFor(b => b.PendingPaymentExpireAt, expiresAt);
        }
        public Faker<BookingEntity> WithEmptyTitle()
        {
            return thisFaker.RuleFor(b => b.Title, string.Empty);
        }
        public Faker<BookingEntity> WithTooLargeTitle()
        {
            return thisFaker.RuleFor(b => b.Title, gen => gen.Random.String2(BookingEntityConstaints.TitleMaxLength + 1));
        }
        public Faker<BookingEntity> WithInvalidBookingPeriod()
        {
            thisFaker.RuleFor(b => b.StartTime, DateTimeOffset.UtcNow);
            return thisFaker.RuleFor(b => b.EndTime, DateTimeOffset.UtcNow.AddHours(-1));
        }
        public Faker<BookingEntity> Cancelled()
        {
            return thisFaker.RuleFor(b => b.Cancelled, true);
        }
    }
}