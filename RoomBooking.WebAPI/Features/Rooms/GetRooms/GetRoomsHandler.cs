using Mediator;
using RoomBooking.Data;
using Microsoft.EntityFrameworkCore;
using RoomBooking.WebAPI.Features.Rooms.Dtos;

namespace RoomBooking.WebAPI.Features.Rooms.GetRooms;

public sealed class GetRoomsHandler(AppDbContext thisDbContext) : IQueryHandler<GetRoomsQuery, IEnumerable<RoomDto>>
{
    #region IQueryHandler<GetRoomsQuery, IEnumerable<RoomDto>>
    public async ValueTask<IEnumerable<RoomDto>> Handle(GetRoomsQuery query, CancellationToken cancellationToken)
    {
        return await thisDbContext.Rooms.AsNoTracking().Select(RoomDto.Projection).ToArrayAsync(cancellationToken);
    }
    #endregion
}