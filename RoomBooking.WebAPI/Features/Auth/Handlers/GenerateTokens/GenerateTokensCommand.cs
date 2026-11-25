using ErrorOr;
using Mediator;
using RoomBooking.Data.Entities.Users;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

public sealed record class GenerateTokensCommand(UserEntity User) : ICommand<ErrorOr<GenerateTokensResponse>>;