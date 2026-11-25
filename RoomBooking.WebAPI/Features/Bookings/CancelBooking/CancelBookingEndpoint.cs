using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Extensions;

namespace RoomBooking.WebAPI.Features.Bookings.CancelBooking;

public static class CancelBookingEndpoint
{
    public const string UrlPath = "/api/bookings/{id:guid}";

    private static async Task<IResult> CancelBooking(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> errorOrNothing = await mediator.Send(new CancelBookingCommand(id), cancellationToken);
        return errorOrNothing.ToHttpResult();
    }
    public static void MapCancelBookingEndpoint(this WebApplication app)
    {
        RouteHandlerBuilder builder = app.MapPatch(UrlPath, CancelBooking);
        builder.WithName(nameof(CancelBooking));
        builder.Produces(StatusCodes.Status204NoContent);
        builder.ProducesProblem(StatusCodes.Status404NotFound);
        builder.AddAuthorizedBehaviorProductionProblems();
        builder.RequireAuthorization();
    }
    public static async ValueTask<HttpResponseMessage> SendCancelBooking(
        this HttpClient httpClient,
        CancelBookingCommand command,
        CancellationToken cancellationToken
    )
    {
        string url = UrlPath.Replace("{id:guid}", command.BookingId.ToString(), StringComparison.Ordinal);
        return await httpClient.PatchAsync(url, content: null, cancellationToken);
    }
}