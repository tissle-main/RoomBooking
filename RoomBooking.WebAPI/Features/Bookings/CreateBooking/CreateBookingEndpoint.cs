using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Extensions;
using RoomBooking.WebAPI.Behaviors.Validation;
using RoomBooking.WebAPI.Features.Bookings.Dtos;

namespace RoomBooking.WebAPI.Features.Bookings.CreateBooking;

public static class CreateBookingEndpoint
{
    public const string UrlPath = "/api/bookings/new";

    private static async Task<IResult> CreateBooking(
        [FromBody] CreateBookingCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<BookingDto> errorOrBooking = await mediator.Send(command, cancellationToken);
        return errorOrBooking.ToHttpResult();
    }
    public static void MapCreateBookingEndpoint(this WebApplication app)
    {
        RouteHandlerBuilder builder = app.MapPost(UrlPath, CreateBooking);
        builder.WithName(nameof(CreateBooking));
        builder.Produces<BookingDto>(StatusCodes.Status200OK);
        builder.ProducesProblem(StatusCodes.Status404NotFound);
        builder.ProducesProblem(StatusCodes.Status409Conflict);
        builder.AddValidationBehaviorProductionProblems();
    }

    public static async ValueTask<HttpResponseMessage> SendCreateBooking(
        this HttpClient httpClient,
        CreateBookingCommand command,
        CancellationToken cancellationToken
    )
    {
        return await httpClient.PostAsJsonAsync(UrlPath, command, cancellationToken);
    }
    public static async ValueTask<(HttpResponseMessage Message, BookingDto? Booking)> SendCreateBooking2(
        this HttpClient httpClient,
        CreateBookingCommand command,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage message = await httpClient.SendCreateBooking(command, cancellationToken);
        if (message.IsSuccessStatusCode)
        {
            BookingDto? booking = await message.Content.ReadFromJsonAsync<BookingDto>(cancellationToken);
            return (message, booking);
        }
        return (message, null);
    }
}