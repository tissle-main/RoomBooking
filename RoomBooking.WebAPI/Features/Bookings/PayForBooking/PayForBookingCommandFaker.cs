using Bogus;

namespace RoomBooking.WebAPI.Features.Bookings.PayForBooking;

public static class PayForBookingCommandFaker
{
    extension(Faker<PayForBookingCommand> thisFaker)
    {
        public Faker<PayForBookingCommand> Valid()
        {
            return thisFaker.CustomInstantiator(gen => new PayForBookingCommand(
                Guid.NewGuid(),
                gen.Random.Decimal(0, 1000),
                gen.Random.String2(3, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")
            ));
        }
        public Faker<PayForBookingCommand> WithBookingId(Guid bookingId)
        {
            return thisFaker.RuleFor(c => c.BookingId, bookingId);
        }
        public Faker<PayForBookingCommand> WithNegativePrice()
        {
            return thisFaker.RuleFor(c => c.Price, -1m);
        }
    }
}
