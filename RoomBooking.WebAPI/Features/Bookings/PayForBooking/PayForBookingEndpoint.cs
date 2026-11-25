using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Behaviors.Authorized;
using RoomBooking.WebAPI.Behaviors.Validation;
using RoomBooking.WebAPI.Extensions;

namespace RoomBooking.WebAPI.Features.Bookings.PayForBooking;

public static class PayForBookingEndpoint
{
    public const string UrlPath = "/api/bookings/pay";

    private static async Task<IResult> PayForBooking(
        [FromBody] PayForBookingCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> errorOrNothing = await mediator.Send(command, cancellationToken);
        return errorOrNothing.ToHttpResult();
    }
    public static void MapPayForBookingEndpoint(this WebApplication app)
    {
        RouteHandlerBuilder builder = app.MapPost(UrlPath, PayForBooking);
        builder.WithName(nameof(PayForBooking));
        builder.Produces(StatusCodes.Status204NoContent);
        builder.ProducesProblem(StatusCodes.Status404NotFound);
        builder.ProducesProblem(StatusCodes.Status409Conflict);
        builder.AddValidationBehaviorProductionProblems();
        builder.AddAuthorizedBehaviorProductionProblems();
        builder.RequireAuthorization();
    }
    public static async ValueTask<HttpResponseMessage> SendPayForBooking(
        this HttpClient httpClient,
        PayForBookingCommand command,
        CancellationToken cancellationToken
    )
    {
        return await httpClient.PostAsJsonAsync(UrlPath, command, cancellationToken);
    }
}
