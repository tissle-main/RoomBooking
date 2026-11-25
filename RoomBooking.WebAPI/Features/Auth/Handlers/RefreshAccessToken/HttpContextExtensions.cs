using RoomBooking.WebAPI.Features.Auth.Dtos.RefreshTokens;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;

public static class HttpContextExtensions
{
    public const string RefreshTokenKey = "refresh-token";

    public static string? GetRefreshToken(this HttpContext httpContext)
    {
        return httpContext.Request.Cookies[RefreshTokenKey];
    }
    public static void AddRefreshToken(this HttpContext httpContext, RefreshTokenDto refreshToken)
    {
        httpContext.Response.Cookies.Append(RefreshTokenKey, refreshToken.Value, new CookieOptions()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = refreshToken.ExpiresAt,
            Path = RefreshAccessTokenEndpoint.UrlPath
        });
    }
}