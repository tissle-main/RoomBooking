using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using RoomBooking.Data.Entities.Users;
using RoomBooking.WebAPI.Features.Auth.Dtos.Users;
using RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;
using RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;
using LoginResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;

public sealed class LoginUserHandler(
    SignInManager<UserEntity> thisSignInManager,
    IHttpContextAccessor thisHttpContextAccessor,
    IMediator thisMediator
) : ICommandHandler<LoginUserCommand, ErrorOr<LoginUserResponse>>
{
    #region ICommandHandler<LoginUserCommand, ErrorOr<LoginUserResponse>>
    public async ValueTask<ErrorOr<LoginUserResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        if(await thisSignInManager.UserManager.FindByEmailAsync(command.Email) is not UserEntity user)
        {
            return AuthErrors.UserNotFound(command.Email);
        }
        LoginResult result = await thisSignInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if(!result.Succeeded)
        {
            return result.ToError();
        }

        ErrorOr<GenerateTokensResponse> errorOrTokens = await thisMediator.Send(new GenerateTokensCommand(user), cancellationToken);
        return errorOrTokens.Then(tokens =>
        {
            thisHttpContextAccessor.HttpContext!.AddRefreshToken(tokens.RefreshToken);
            return new LoginUserResponse(UserDto.FromSource(user), tokens.AccessToken);
        });    
    }
    #endregion
}