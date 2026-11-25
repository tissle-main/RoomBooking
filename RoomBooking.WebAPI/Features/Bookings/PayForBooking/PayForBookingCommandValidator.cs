using FluentValidation;

namespace RoomBooking.WebAPI.Features.Bookings.PayForBooking;

public sealed class PayForBookingCommandValidator : AbstractValidator<PayForBookingCommand>
{
    public PayForBookingCommandValidator()
    {
        base.RuleFor(c => c.Price).GreaterThanOrEqualTo(0m);
    }
}
