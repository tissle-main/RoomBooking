using Facet;
using RoomBooking.Data.Entities.Rooms;

namespace RoomBooking.WebAPI.Features.Rooms.Dtos;

[Facet(
    sourceType: typeof(RoomEntity),
    exclude: [
        nameof(RoomEntity.Bookings)
    ]
)]
public sealed partial class RoomDto;