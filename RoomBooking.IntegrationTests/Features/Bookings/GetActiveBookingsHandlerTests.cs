using Bogus;
using TUnit.Assertions.Enums;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Users;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.IntegrationTests.Seeders;
using RoomBooking.WebAPI.Features.Bookings.Dtos;
using RoomBooking.IntegrationTests.Features.Auth;
using RoomBooking.IntegrationTests.Features.Rooms;
using RoomBooking.WebAPI.Features.Bookings.GetActiveBookings;

namespace RoomBooking.IntegrationTests.Features.Bookings;

[DependsOn<RoomSeedingTests>]
[DependsOn<LoginUserHandlerTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class GetActiveBookingsHandlerTests(AppFixture thisApp)
{
    private static Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldFail_WhenUserIsUnauthorized(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        GetActiveBookingsQuery query = new(Guid.NewGuid(), DateOnly.FromDateTime(DateTime.Today));

        //Act
        using HttpResponseMessage message = await thisApp.HttpClient.SendGetActiveBookings(query, cancellationToken);

        //Assert
        await Assert.That(message.StatusCode).IsEqualTo(System.Net.HttpStatusCode.Unauthorized);
    }

    [Test]
    public async ValueTask Handler_ShouldReturnActiveBookingsForRoom(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);
        (UserEntity User, string Password)[] users = (await thisApp.AddUsers2Async(cancellationToken, min: 2, max: 2)).ToArray();
        await thisApp.AuthenticateAsAsync(users[0].User, users[0].Password, cancellationToken);
        Guid roomId = await thisApp.ExecuteDbContextAsync(async db =>
        {
            Guid[] roomIds = await db.Rooms.Select(r => r.Id).ToArrayAsync(cancellationToken);
            return Faker.PickRandom(roomIds);
        });
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await new Faker<BookingEntity>().Valid(roomId).WithUserId(users[0].User.Id).SeedDatabaseAsync(db, cancellationToken);
            await new Faker<BookingEntity>().Valid(roomId).WithUserId(users[0].User.Id).Cancelled().SeedDatabaseAsync(db, cancellationToken);
            await new Faker<BookingEntity>().Valid(roomId).WithUserId(users[1].User.Id).SeedDatabaseAsync(db, cancellationToken);
        });
        GetActiveBookingsQuery query = new(roomId, DateOnly.FromDateTime(DateTime.Today));
        DateTime dayStart = query.Day.ToDateTime(TimeOnly.MinValue).ToUniversalTime();
        DateTime dayEnd = query.Day.AddDays(1).ToDateTime(TimeOnly.MinValue).ToUniversalTime();

        //Act
        (HttpResponseMessage message, BookingDto[]? bookings) = await thisApp.HttpClient.SendGetActiveBookings2(query, cancellationToken);
        await Assert.That(message.IsSuccessStatusCode).IsTrue();
        await Assert.That(bookings).IsNotNull();

        //Assert
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            BookingDto[] expectedBookings = await db.Bookings.AsNoTracking().Where(
                b => b.RoomId == roomId
                            && b.UserId == users[0].User.Id
                            && !b.Cancelled
                            && b.StartTime < dayEnd
                        && b.EndTime > dayStart
            ).Select(BookingDto.Projection).OrderBy(b => b.StartTime).ToArrayAsync(cancellationToken);
            await Assert.That(expectedBookings).IsEquivalentTo(bookings!, CollectionOrdering.Matching);
        });
    }
}