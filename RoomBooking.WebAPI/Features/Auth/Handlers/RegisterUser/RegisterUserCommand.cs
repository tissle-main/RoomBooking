using ErrorOr;
using Mediator;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RegisterUser;

public sealed record class RegisterUserCommand(string Email, string Password) : ICommand<ErrorOr<Unit>>;