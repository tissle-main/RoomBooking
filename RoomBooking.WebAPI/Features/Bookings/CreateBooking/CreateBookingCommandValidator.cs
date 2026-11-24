using FluentValidation;
using RoomBooking.Data.Entities.Bookings;

namespace RoomBooking.WebAPI.Features.Bookings.CreateBooking;

public sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        base.RuleFor(c => c.Title).NotEmpty().MaximumLength(BookingEntityConstaints.TitleMaxLength);
        base.RuleFor(c => c.StartTime).LessThan(c => c.EndTime);
    }
}