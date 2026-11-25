using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Users;
using RoomBooking.IntegrationTests.Seeders;
using RoomBooking.WebAPI.Features.Auth.Handlers.RegisterUser;

namespace RoomBooking.IntegrationTests.Features.Auth;

[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class RegisterUserHandlerTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldCreateUser(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            UserEntity? user = await db.Users.SingleOrDefaultAsync(cancellationToken);
            await Assert.That(user).IsNotNull();
            await Assert.That(user.Email).IsEqualTo(command.Email);
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUserExists(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = await thisApp.AddUsersAndPickRandomAsync(cancellationToken);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithInvalidEmail().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordTooShort(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithTooShortPassword().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithPasswordWithoutDigits().Generate();

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendRegisterUserAsync(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}