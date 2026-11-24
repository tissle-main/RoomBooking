using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Features.Rooms.Dtos;

namespace RoomBooking.WebAPI.Features.Rooms.GetRooms;

public static class GetRoomsEndpoint
{
    public const string UrlPath = "/api/rooms";

    private static async Task<IResult> GetRooms([FromServices] IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new GetRoomsQuery(), cancellationToken));
    }
    public static void MapGetRoomsEndpoint(this WebApplication app)
    {
        RouteHandlerBuilder builder = app.MapGet(UrlPath, GetRooms);
        builder.WithName(nameof(GetRooms));
        builder.Produces<IEnumerable<RoomDto>>();
    }
    public static async ValueTask<RoomDto[]?> SendGetRoomsRequest(this HttpClient httpClient, CancellationToken cancellationToken)
    {
        return await httpClient.GetFromJsonAsync<RoomDto[]>(UrlPath, cancellationToken);
    }
}