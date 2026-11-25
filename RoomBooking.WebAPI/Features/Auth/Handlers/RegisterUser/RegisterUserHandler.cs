using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Identity;
using RoomBooking.Data.Entities.Users;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RegisterUser;

public sealed class RegisterUserHandler(UserManager<UserEntity> thisUserManager) : ICommandHandler<RegisterUserCommand, ErrorOr<Unit>>
{
    #region ICommandHandler<RegisterUserCommand, ErrorOr<Unit>>
    public async ValueTask<ErrorOr<Unit>> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
    {
        if(await thisUserManager.FindByEmailAsync(command.Email) is not null)
        {
            return AuthErrors.UserExists(command.Email);
        }
        UserEntity user = new()
        {
            Email = command.Email,
            UserName = command.Email
        };

        IdentityResult result = await thisUserManager.CreateAsync(user, command.Password);
        if(!result.Succeeded)
        {
            return result.ToErrors().ToList();
        }
        return Unit.Value;
    }
    #endregion
}