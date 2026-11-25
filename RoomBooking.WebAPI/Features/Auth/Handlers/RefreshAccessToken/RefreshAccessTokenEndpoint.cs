using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Extensions;
using RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;

public static class RefreshAccessTokenEndpoint
{
    public const string UrlPath = "/api/auth/refresh-token";

    public static async Task<IResult> RefreshAccessToken(
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<RefreshAccessTokenResponse> result = await mediator.Send(new RefreshAccessTokenCommand(), cancellationToken);
        return result.ToHttpResult();
    }
    public static RouteHandlerBuilder AddRefreshAccessTokenProductionProblems(this RouteHandlerBuilder builder)
    {
        builder.ProducesProblem(StatusCodes.Status401Unauthorized);
        return builder.AddGenerateTokensProductionProblems();
    }
    public static void MapRefreshAccessTokenEndpoint(this IEndpointRouteBuilder builder)
    {
        RouteHandlerBuilder route = builder.MapPut(UrlPath, RefreshAccessToken);
        route.WithName(nameof(RefreshAccessToken));
        route.Produces<RefreshAccessTokenResponse>(StatusCodes.Status200OK);
        route.AddRefreshAccessTokenProductionProblems();
    }
    public static async ValueTask<HttpResponseMessage> SendRefreshAccessTokenAsync(
        this HttpClient httpClient,
        CancellationToken cancellationToken
    )
    {
        return await httpClient.PutAsync(UrlPath, null, cancellationToken);
    }
    public static async ValueTask<(HttpResponseMessage Message, RefreshAccessTokenResponse? Response)> SendRefreshAccessTokenAsync2(
        this HttpClient httpClient,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage message = await httpClient.SendRefreshAccessTokenAsync(cancellationToken);
        if(message.IsSuccessStatusCode)
        {
            RefreshAccessTokenResponse? response = await message.Content.ReadFromJsonAsync<RefreshAccessTokenResponse>(cancellationToken);
            return (message, response);
        }
        return (message, null);
    }
}