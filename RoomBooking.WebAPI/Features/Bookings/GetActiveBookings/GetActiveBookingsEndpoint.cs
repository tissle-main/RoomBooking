using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using RoomBooking.WebAPI.Features.Bookings.Dtos;

namespace RoomBooking.WebAPI.Features.Bookings.GetActiveBookings;

public static class GetActiveBookingsEndpoint
{
    public const string UrlPath = "/api/bookings/active";

    private static async Task<IResult> GetActiveBookings(
        [FromQuery] Guid roomId,
        [FromQuery] DateOnly day,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        GetActiveBookingsQuery query = new(roomId, day);
        ErrorOr<IEnumerable<BookingDto>> errorOrBookings = await mediator.Send(query, cancellationToken);
        return errorOrBookings.ToHttpResult();
    }
    public static void MapGetActiveBookingsEndpoint(this WebApplication app)
    {
        RouteHandlerBuilder builder = app.MapGet(UrlPath, GetActiveBookings);
        builder.WithName(nameof(GetActiveBookings));
        builder.Produces<IEnumerable<BookingDto>>(StatusCodes.Status200OK);
        builder.ProducesProblem(StatusCodes.Status404NotFound);
        builder.AddAuthorizedBehaviorProductionProblems();
        builder.RequireAuthorization();
    }

    private static string MakeSendableUrl(GetActiveBookingsQuery query)
    {
        IEnumerable<KeyValuePair<string, string?>> queryParams = [
            new KeyValuePair<string, string?>(nameof(query.RoomId), query.RoomId.ToString()),
            new KeyValuePair<string, string?>(nameof(query.Day), query.Day.ToString())
        ];
        return QueryHelpers.AddQueryString(UrlPath, queryParams);
    }
    public static async ValueTask<HttpResponseMessage> SendGetActiveBookings(
        this HttpClient httpClient,
        GetActiveBookingsQuery query,
        CancellationToken cancellationToken
    )
    {
        return await httpClient.GetAsync(MakeSendableUrl(query), cancellationToken);
    }
    public static async ValueTask<(HttpResponseMessage Message, BookingDto[]? Bookings)> SendGetActiveBookings2(
        this HttpClient httpClient,
        GetActiveBookingsQuery query,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage message = await httpClient.SendGetActiveBookings(query, cancellationToken);
        if(message.IsSuccessStatusCode)
        {
            BookingDto[]? dtos = await message.Content.ReadFromJsonAsync<BookingDto[]>(cancellationToken);
            return (message, dtos);
        }
        return (message, null);
    }
}