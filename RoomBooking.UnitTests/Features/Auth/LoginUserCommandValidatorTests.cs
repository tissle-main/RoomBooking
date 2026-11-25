using Bogus;
using FluentValidation.TestHelper;
using RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;

namespace RoomBooking.UnitTests.Features.Auth;

public sealed class LoginUserCommandValidatorTests
{
    public LoginUserCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithInvalidEmail().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordTooShort(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithTooShortPassword().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        LoginUserCommand command = new Faker<LoginUserCommand>().Valid().WithPasswordWithoutDigits().Generate();

        //Act
        TestValidationResult<LoginUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}