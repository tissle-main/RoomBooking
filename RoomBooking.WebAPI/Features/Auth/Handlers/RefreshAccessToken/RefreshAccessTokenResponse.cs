using RoomBooking.WebAPI.Features.Auth.Dtos.Users;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;

public sealed record class RefreshAccessTokenResponse(UserDto User, string AccessToken);