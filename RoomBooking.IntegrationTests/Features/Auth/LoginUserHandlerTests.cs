using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Users;
using RoomBooking.IntegrationTests.Seeders;
using RoomBooking.Data.Entities.RefreshTokens;
using RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;

namespace RoomBooking.IntegrationTests.Features.Auth;

[DependsOn<RegisterUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class LoginUserHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldCreateRefreshToken(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, string password) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        LoginUserCommand command = new(user.Email!, password);

        //Act
        (HttpResponseMessage message, LoginUserResponse? response) = await thisApp.HttpClient.SendLoginUserAsync2(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.OK);
        await Assert.That(response).IsNotNull();
        await Assert.That(response!.User.Email).IsEqualTo(command.Email);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RefreshTokenEntity? refreshToken = await db.RefreshTokens.SingleOrDefaultAsync(cancellationToken);
            await Assert.That(refreshToken).IsNotNull();
            await Assert.That(refreshToken!.UserId).IsEqualTo(user.Id);
            await Assert.That(refreshToken.ExpiresAt).IsGreaterThan(DateTime.UtcNow);
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUserNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (_, string password) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithPassword(password).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndPickRandomAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithEmail(user.Email!).Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.BadRequest);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithInvalidEmail().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordTooShort(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithTooShortPassword().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithPasswordWithoutDigits().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendLoginUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}