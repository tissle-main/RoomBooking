using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;
using RoomBooking.WebAPI.Extensions;
using RoomBooking.Data.Entities.Users;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

public static class GenerateTokensEndpoint
{
    public const string UrlPath = "/api/auth/generate-tokens";

    public static async Task<IResult> GenerateTokens(
        [FromBody] UserEntity user,
        [FromServices] IMediator mediator,
        CancellationToken cancellationToken
    )
    {
        ErrorOr<GenerateTokensResponse> errorOrUnit = await mediator.Send(new GenerateTokensCommand(user), cancellationToken);
        return errorOrUnit.ToHttpResult();
    }
    public static RouteHandlerBuilder AddGenerateTokensProductionProblems(this RouteHandlerBuilder builder)
    {
        return builder;
    }
    public static void MapGenerateTokensEndpoint(this IEndpointRouteBuilder builder)
    {
        RouteHandlerBuilder route = builder.MapPost(UrlPath, GenerateTokens);
        route.WithName(nameof(GenerateTokens));
        route.Produces<GenerateTokensResponse>(StatusCodes.Status200OK);
        route.AddGenerateTokensProductionProblems();
    }
    public static async ValueTask<HttpResponseMessage> SendGenerateTokensAsync(
        this HttpClient httpClient,
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        return await httpClient.PostAsJsonAsync(UrlPath, user, cancellationToken);
    }
    public static async ValueTask<(HttpResponseMessage Message, GenerateTokensResponse? Response)> SendGenerateTokensAsync2(
        this HttpClient httpClient,
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage message = await httpClient.SendGenerateTokensAsync(user, cancellationToken);
        if(message.IsSuccessStatusCode)
        {
            GenerateTokensResponse? response = await message.Content.ReadFromJsonAsync<GenerateTokensResponse>(cancellationToken);
            return (message, response);
        }
        return (message, null);
    }
}