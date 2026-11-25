using Bogus;
using System.Net;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Features.Bookings.Dtos;
using RoomBooking.IntegrationTests.Features.Rooms;
using RoomBooking.WebAPI.Features.Bookings.CancelBooking;

namespace RoomBooking.IntegrationTests.Features.Bookings;

[DependsOn<RoomSeedingTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class CancelBookingHandlerTests(AppFixture thisApp)
{
    private static Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_CancelBooking(CancellationToken cancellationToken)
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
    public async ValueTask Handler_ShouldFail_WhenBookingNotFound(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<BookingEntity>().Valid(roomId).SeedDatabaseAsync(db, cancellationToken);
        });
        CancelBookingCommand command = new(Guid.NewGuid());

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendCancelBooking(command, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(HttpStatusCode.NotFound);
    }
}