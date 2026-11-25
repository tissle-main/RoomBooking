using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Users;
using RoomBooking.IntegrationTests.Seeders;
using RoomBooking.Data.Entities.RefreshTokens;
using RoomBooking.WebAPI.Features.Auth.Handlers.GenerateTokens;

namespace RoomBooking.IntegrationTests.Features.Auth;

[DependsOn<RegisterUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class GenerateTokensHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldCreateAndReturnRefreshToken(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);

        //Act
        (HttpResponseMessage message, GenerateTokensResponse? response) = await thisApp.HttpClient.SendGenerateTokensAsync2(user, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? refreshToken = await db.RefreshTokens.SingleOrDefaultAsync(cancellationToken);
            await Assert.That(refreshToken).IsNotNull();
            await Assert.That(refreshToken!.UserId).IsEqualTo(user.Id);
            await Assert.That(refreshToken.ExpiresAt).IsGreaterThan(DateTime.UtcNow);
            await Assert.That(refreshToken.Value).IsEqualTo(response!.RefreshToken.Value);
        });
    }
}