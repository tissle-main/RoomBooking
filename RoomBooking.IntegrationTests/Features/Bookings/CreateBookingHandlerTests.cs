using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Features.Bookings.Dtos;
using RoomBooking.IntegrationTests.Features.Rooms;
using RoomBooking.WebAPI.Features.Bookings.CreateBooking;

namespace RoomBooking.IntegrationTests.Features.Bookings;

[DependsOn<RoomSeedingTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class CreateBookingHandlerTests(AppFixture thisApp)
{
    private static Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldCreateBooking(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid(roomId));

        //Act
        (HttpResponseMessage message, BookingDto? booking) = await thisApp.HttpClient.SendCreateBooking2(command, cancellationToken);
        await Assert.That(message.IsSuccessStatusCode).IsTrue();
        await Assert.That(booking).IsNotNull();

        //Assert
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            BookingEntity? bookingEntity = await db.Bookings.SingleOrDefaultAsync(cancellationToken);
            await Assert.That(bookingEntity).IsNotNull();
            await Assert.That(BookingDto.FromSource(bookingEntity)).IsEquivalentTo(booking);
            await Assert.That(bookingEntity.Title).IsEqualTo(command.Title);
            await Assert.That(bookingEntity.StartTime).IsEqualTo(command.StartTime.ToUniversalTime());
            await Assert.That(bookingEntity.EndTime).IsEqualTo(command.EndTime.ToUniversalTime());
        });
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(-1)]
    public async ValueTask Handler_ShouldFail_WhenOverlaps(int hourOffset, CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        List<BookingEntity> bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<BookingEntity>().Valid(roomId).SeedDatabaseAsync(db, cancellationToken);
        });
        BookingEntity booking = Faker.PickRandom(bookings);
        var command = new CreateBookingCommand(new Faker<BookingEntity>().Valid(booking.RoomId)) with
        {
            StartTime = booking.StartTime.AddHours(hourOffset),
            EndTime = booking.EndTime.AddHours(hourOffset)
        };

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateBooking(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Conflict);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenTitleIsEmpty(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid(roomId).WithEmptyTitle());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateBooking(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenTitleIsTooLarge(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid(roomId).WithTooLargeTitle());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateBooking(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenBookingPeriodIsInvalid(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        CreateBookingCommand command = new(new Faker<BookingEntity>().Valid(roomId).WithInvalidBookingPeriod());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCreateBooking(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.UnprocessableEntity);
    }
}