using ErrorOr;
using Mediator;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;

public sealed record class LoginUserCommand(string Email, string Password) : ICommand<ErrorOr<LoginUserResponse>>;