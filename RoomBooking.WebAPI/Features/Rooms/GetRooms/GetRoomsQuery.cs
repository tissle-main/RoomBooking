using Mediator;
using RoomBooking.WebAPI.Features.Rooms.Dtos;

namespace RoomBooking.WebAPI.Features.Rooms.GetRooms;

public sealed record class GetRoomsQuery : IQuery<IEnumerable<RoomDto>>;