using ErrorOr;
using Mediator;
using RoomBooking.Data;
using Microsoft.Extensions.Options;
using RoomBooking.Data.Entities.RefreshTokens;
using RoomBooking.WebAPI.Features.Auth.Options;
using RoomBooking.WebAPI.Features.Auth.Services;
using RoomBooking.WebAPI.Features.Auth.Dtos.RefreshTokens;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

public sealed class GenerateTokensHandler(
    AppDbContext thisDbContext,
    IOptionsSnapshot<RefreshTokenOptions> tokenOptions,
    IAccessTokenGenerator thisAccessTokenGenerator,
    IRefreshTokenGenerator thisRefreshTokenGenerator
) : ICommandHandler<GenerateTokensCommand, ErrorOr<GenerateTokensResponse>>
{
    #region ICommandHandler<GenerateTokensCommand, ErrorOr<GenerateTokensResponse>>
    public async ValueTask<ErrorOr<GenerateTokensResponse>> Handle(GenerateTokensCommand command, CancellationToken cancellationToken)
    {
        string refreshToken = await thisRefreshTokenGenerator.GenerateTokenAsync(RefreshTokenEntityConstraints.ValueMaxLength, cancellationToken);
        RefreshTokenEntity entity = new()
        {
            Value = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(tokenOptions.Value.ExpireDays),
            UserId = command.User.Id,
        };

        await thisDbContext.RefreshTokens.AddAsync(entity, cancellationToken);
        await thisDbContext.SaveChangesAsync(cancellationToken);

        string accessToken = await thisAccessTokenGenerator.GenerateTokenAsync(command.User, cancellationToken);
        return new GenerateTokensResponse(accessToken, RefreshTokenDto.FromSource(entity));
    }
    #endregion
}