using ErrorOr;
using Mediator;
using RoomBooking.Data;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.RefreshTokens;
using RoomBooking.WebAPI.Features.Auth.Dtos.Users;
using RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;

public sealed class RefreshAccessTokenHandler(
    AppDbContext thisDbContext,
    IHttpContextAccessor thisHttpContextAccessor,
    IMediator thisMediator
) : ICommandHandler<RefreshAccessTokenCommand, ErrorOr<RefreshAccessTokenResponse>>
{
    #region ICommandHandler<RefreshAccessTokenCommand, ErrorOr<RefreshAccessTokenResponse>>
    public async ValueTask<ErrorOr<RefreshAccessTokenResponse>> Handle(RefreshAccessTokenCommand command, CancellationToken cancellationToken)
    {
        if(thisHttpContextAccessor.HttpContext!.GetRefreshToken() is not string refreshToken)
        {
            return Error.Unauthorized();
        }
        RefreshTokenEntity? entity = await thisDbContext.RefreshTokens.Include(e => e.User).FirstOrDefaultAsync(
            e => e.Value == refreshToken,
            cancellationToken
        );
        if(entity is null || DateTime.UtcNow > entity.ExpiresAt)
        {
            return Error.Unauthorized();
        }

        thisDbContext.RefreshTokens.Remove(entity);
        await thisDbContext.SaveChangesAsync(cancellationToken);

        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(entity.User!), cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            thisHttpContextAccessor.HttpContext!.AddRefreshToken(tokens.RefreshToken);
            return new RefreshAccessTokenResponse(UserDto.FromSource(entity.User!), tokens.AccessToken);
        });
    }
    #endregion
}