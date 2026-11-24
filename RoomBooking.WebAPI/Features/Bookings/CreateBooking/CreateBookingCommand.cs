using Facet;
using ErrorOr;
using Mediator;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Features.Bookings.Dtos;

namespace RoomBooking.WebAPI.Features.Bookings.CreateBooking;

[Facet(
    sourceType: typeof(BookingEntity),
    exclude: [
        nameof(BookingEntity.Room),
        nameof(BookingEntity.Id),
        nameof(BookingEntity.Cancelled)
    ],
    GenerateToSource = true
)]
public sealed partial record class CreateBookingCommand : ICommand<ErrorOr<BookingDto>>;