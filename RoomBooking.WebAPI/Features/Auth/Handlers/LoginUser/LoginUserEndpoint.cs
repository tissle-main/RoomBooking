using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Extensions;
using RoomBooking.WebAPI.Behaviors.Validation;
using RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;

public static class LoginUserEndpoint
{
    public const string UrlPath = "/api/auth/login";

    public static async Task<IResult> LoginUser(
        [FromBody] LoginUserCommand command,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<LoginUserResponse> result = await mediator.Send(command, cancellationToken);
        return result.ToHttpResult();
    }
    public static RouteHandlerBuilder AddLoginUserProductionProblems(this RouteHandlerBuilder builder)
    {
        builder.ProducesProblem(StatusCodes.Status400BadRequest);
        builder.ProducesProblem(StatusCodes.Status404NotFound);
        builder.AddValidationBehaviorProductionProblems();
        return builder.AddGenerateTokensProductionProblems();
    }
    public static void MapLoginUserEndpoint(this IEndpointRouteBuilder builder)
    {
        RouteHandlerBuilder route = builder.MapPost(UrlPath, LoginUser);
        route.WithName(nameof(LoginUser));
        route.Produces<LoginUserResponse>(StatusCodes.Status200OK);
        route.AddLoginUserProductionProblems();
    }
    public static async ValueTask<HttpResponseMessage> SendLoginUserAsync(
        this HttpClient httpClient,
        LoginUserCommand command,
        CancellationToken cancellationToken
    )
    {
        return await httpClient.PostAsJsonAsync(UrlPath, command, cancellationToken);
    }
    public static async ValueTask<(HttpResponseMessage Message, LoginUserResponse? Response)> SendLoginUserAsync2(
        this HttpClient httpClient,
        LoginUserCommand command,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage message = await httpClient.SendLoginUserAsync(command, cancellationToken);
        if(message.IsSuccessStatusCode)
        {
            LoginUserResponse? response = await message.Content.ReadFromJsonAsync<LoginUserResponse>(cancellationToken);
            return (message, response);
        }
        return (message, null);
    }
}