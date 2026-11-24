using Microsoft.EntityFrameworkCore;
using RoomBooking.WebAPI.Features.Rooms.Dtos;
using RoomBooking.WebAPI.Features.Rooms.GetRooms;

namespace RoomBooking.IntegrationTests.Features.Rooms.GetRooms;

[DependsOn<RoomSeedingTests>]
[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class GetRoomsTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Handler_ShouldReturnAllRooms(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);

        //Act
        RoomDto[]? rooms = await thisApp.HttpClient.SendGetRoomsRequest(cancellationToken);
        await Assert.That(rooms).IsNotNull();

        //Assert
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            RoomDto[] expectedRooms = await db.Rooms.AsNoTracking().Select(RoomDto.Projection).ToArrayAsync(cancellationToken);
            await Assert.That(rooms).IsEquivalentTo(expectedRooms);
        });
    }
}