using Facet;
using RoomBooking.Data.Entities.Bookings;

namespace RoomBooking.WebAPI.Features.Bookings.Dtos;

[Facet(
    sourceType: typeof(BookingEntity),
    exclude: [
        nameof(BookingEntity.Room),
    ]
)]
public sealed partial class BookingDto;