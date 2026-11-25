using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Users;
using RoomBooking.IntegrationTests.Seeders;
using RoomBooking.Data.Entities.RefreshTokens;
using RoomBooking.WebAPI.Features.Auth.Handlers.RefreshAccessToken;

namespace RoomBooking.IntegrationTests.Features.Auth;

[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class RefreshAccessTokenHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldReplaceRefreshToken(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        RefreshTokenEntity oldRefreshToken = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await db.RefreshTokens.SingleAsync(cancellationToken);
        });

        //Act
        (HttpResponseMessage message, RefreshAccessTokenResponse? response) = await thisApp.HttpClient.SendRefreshAccessTokenAsync2(cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();
        await Assert.That(response!.User.Email).IsEqualTo(user.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? newRefreshToken = await db.RefreshTokens.SingleOrDefaultAsync(cancellationToken);
            await Assert.That(newRefreshToken).IsNotNull();
            await Assert.That(newRefreshToken!.Id).IsNotEqualTo(oldRefreshToken.Id);
            await Assert.That(newRefreshToken.Value).IsNotEqualTo(oldRefreshToken.Value);
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenRefreshTokenNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity entity = await db.RefreshTokens.SingleAsync(cancellationToken);
            db.RefreshTokens.Remove(entity);
            await db.SaveChangesAsync(cancellationToken);
            return entity;
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(cancellationToken);

        //Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenRefreshTokenExpired(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity entity = await db.RefreshTokens.SingleAsync(cancellationToken);
            entity.ExpiresAt = DateTime.UtcNow.AddDays(-1);
            db.RefreshTokens.Update(entity);
            await db.SaveChangesAsync(cancellationToken);
            return entity;
        });

        //Act
        using HttpResponseMessage response = await thisApp.HttpClient.SendRefreshAccessTokenAsync(cancellationToken);

        //Assert
        await Assert.That(response.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }
}