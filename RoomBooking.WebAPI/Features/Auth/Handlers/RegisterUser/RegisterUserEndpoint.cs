using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Extensions;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RegisterUser;

public static class RegisterUserEndpoint
{
    public const string UrlPath = "/api/auth/register";

    public static async Task<IResult> RegisterUser(
        [FromBody] RegisterUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<Unit> errorOrUnit = await mediator.Send(command, cancellationToken);
        return errorOrUnit.ToHttpResult();
    }
    public static RouteHandlerBuilder AddRegisterUserProductionProblems(this RouteHandlerBuilder builder)
    {
        builder.ProducesProblem(StatusCodes.Status400BadRequest);
        builder.ProducesProblem(StatusCodes.Status409Conflict);
        return builder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
    }
    public static void MapRegisterUserEndpoint(this IEndpointRouteBuilder builder)
    {
        RouteHandlerBuilder route = builder.MapPost(UrlPath, RegisterUser);
        route.WithName(nameof(RegisterUser));
        route.Produces(StatusCodes.Status204NoContent);
        route.AddRegisterUserProductionProblems();
    }
    public static async ValueTask<HttpResponseMessage> SendRegisterUserAsync(
        this HttpClient httpClient,
        RegisterUserCommand command,
        CancellationToken cancellationToken
    )
    {
        return await httpClient.PostAsJsonAsync(UrlPath, command, cancellationToken);
    }
}