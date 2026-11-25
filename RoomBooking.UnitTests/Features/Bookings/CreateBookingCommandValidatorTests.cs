using Bogus;
using FluentValidation.TestHelper;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Features.Bookings.Dtos;
using RoomBooking.WebAPI.Features.Bookings.CreateBooking;

namespace RoomBooking.UnitTests.Features.Bookings;

public sealed class CreateBookingCommandValidatorTests
{
    public CreateBookingCommandValidator Validator { get; } = new();

    [Test]
    public async ValueTask Validator_ShouldPass_WhenCommandIsValid(CancellationToken cancellationToken)
    {
        //Arrange
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid().Generate());

        //Act
        TestValidationResult<CreateBookingCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenTitleIsEmpty(CancellationToken cancellationToken)
    {
        //Arrange
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid().WithEmptyTitle().Generate());

        //Act
        TestValidationResult<CreateBookingCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenTitleIsTooLarge(CancellationToken cancellationToken)
    {
        //Arrange
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid().WithTooLargeTitle().Generate());

        //Act
        TestValidationResult<CreateBookingCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(c => c.Title);
    }

    [Test]
    public async ValueTask Validator_ShouldNotPass_WhenBookingPeriodIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid().WithInvalidBookingPeriod().Generate());

        //Act
        TestValidationResult<CreateBookingCommand> result = await Validator.TestValidateAsync(command, null, cancellationToken);

        //Assert
        result.ShouldHaveValidationErrorFor(b => b.StartTime);
    }
}