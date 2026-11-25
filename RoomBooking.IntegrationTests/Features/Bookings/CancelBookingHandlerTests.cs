using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Users;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.IntegrationTests.Seeders;
using RoomBooking.WebAPI.Features.Bookings.Dtos;
using RoomBooking.IntegrationTests.Features.Auth;
using RoomBooking.IntegrationTests.Features.Rooms;
using RoomBooking.WebAPI.Features.Bookings.CancelBooking;

namespace RoomBooking.IntegrationTests.Features.Bookings;

[DependsOn<RoomSeedingTests>]
[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class CancelBookingHandlerTests(AppFixture thisApp)
{
    private static Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_CancelBooking(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        List<BookingEntity> bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<BookingEntity>().Valid(roomId).WithUserId(user.Id).SeedDatabaseAsync(db, cancellationToken);
        });
        Guid bookingId = Faker.PickRandom(bookings).Id;
        CancelBookingCommand command = new(bookingId);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCancelBooking(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NoContent);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            BookingEntity? booking = await db.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);
            await Assert.That(booking).IsNotNull();
            await Assert.That(booking.Cancelled).IsTrue();
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUserIsUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCancelBooking(new CancelBookingCommand(Guid.NewGuid()), cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Unauthorized);
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenBookingBelongsToAnotherUser(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity User, string Password)[] users = (await thisApp.AddUsers2Async(cancellationToken, min: 2, max: 2)).ToArray();
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        List<BookingEntity> bookings = await thisApp.ExecuteDbContextAsync(async db =>
        {
            return await new Faker<BookingEntity>().Valid(roomId).WithUserId(users[0].User.Id).SeedDatabaseAsync(db, cancellationToken, min: 1, max: 1);
        });
        await thisApp.AuthenticateAsAsync(users[1].User, users[1].Password, cancellationToken);

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCancelBooking(new CancelBookingCommand(bookings[0].Id), cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.Forbidden);
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            BookingEntity? booking = await db.Bookings.SingleOrDefaultAsync(b => b.Id == bookings[0].Id, cancellationToken);
            await Assert.That(booking).IsNotNull();
            await Assert.That(booking!.Cancelled).IsFalse();
        });
    }

    [Test]
    public async ValueTask Handler_ShouldFail_WhenBookingNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity user, _) = await thisApp.AddUsers2AndLoginRandomAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<BookingEntity>().Valid(roomId).WithUserId(user.Id).SeedDatabaseAsync(db, cancellationToken);
        });
        CancelBookingCommand command = new(Guid.NewGuid());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCancelBooking(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }
}