using RoomBooking.WebAPI.Features.Auth.Dtos.RefreshTokens;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

public sealed record class GenerateTokensResponse(string AccessToken, RefreshTokenDto RefreshToken);