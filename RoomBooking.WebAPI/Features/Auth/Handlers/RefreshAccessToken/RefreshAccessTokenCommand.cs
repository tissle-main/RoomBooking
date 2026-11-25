using ErrorOr;
using Mediator;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenCommand : ICommand<ErrorOr<RefreshAccessTokenResponse>>;