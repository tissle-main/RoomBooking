using System.Security.Cryptography;

namespace RoomBooking.WebAPI.Features.Auth.Services;

public sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    #region IRefreshTokenGenerator
    public async ValueTask<string> GenerateTokenAsync(int length, CancellationToken cancellationToken)
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(length))[..length];
    }
    #endregion
}