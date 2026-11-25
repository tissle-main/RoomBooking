using Bogus;
using TUnit.Assertions.Enums;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.WebAPI.Features.Bookings.Dtos;
using RoomBooking.IntegrationTests.Features.Rooms;
using RoomBooking.WebAPI.Features.Bookings.GetActiveBookings;

namespace RoomBooking.IntegrationTests.Features.Bookings;

[DependsOn<RoomSeedingTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class GetActiveBookingsHandlerTests(AppFixture thisApp)
{
    private static Faker Faker { get; } = new();

    [Test]
    public async ValueTask Handler_ShouldReturnActiveBookingsForRoom(CancellationToken cancellationToken)
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
            await new Faker<BookingEntity>().Valid(roomId).Cancelled().SeedDatabaseAsync(db, cancellationToken);
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
                        && !b.Cancelled
                        && b.StartTime < dayEnd
                        && b.EndTime > dayStart
            ).Select(BookingDto.Projection).OrderBy(b => b.StartTime).ToArrayAsync(cancellationToken);
            await Assert.That(expectedBookings).IsEquivalentTo(bookings!, CollectionOrdering.Matching);
        });
    }
}