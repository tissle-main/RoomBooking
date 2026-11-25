using Bogus;
using FluentValidation.TestHelper;
using RoomBooking.WebAPI.Features.Auth.Handlers.RegisterUser;

namespace RoomBooking.UnitTests.Features.Auth;

public sealed class RegisterUserCommandValidatorTests
{
    public RegisterUserCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenEmailIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithInvalidEmail().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Email);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordTooShort(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithTooShortPassword().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainUppercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithPasswordWithoutUppercaseLetters().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainLowercaseLetters(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithPasswordWithoutLowercaseLetters().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenPasswordDoNotContainDigits(CancellationToken cancellationToken)
    {
        //Arrange
        RegisterUserCommand command = new Faker<RegisterUserCommand>().Valid().WithPasswordWithoutDigits().Generate();

        //Act
        TestValidationResult<RegisterUserCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Password);
    }
}