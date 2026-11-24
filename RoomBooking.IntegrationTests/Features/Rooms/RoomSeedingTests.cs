using Microsoft.EntityFrameworkCore;

namespace RoomBooking.IntegrationTests.Features.Rooms;

[ClassDataSource<AppFixture>(Shared = SharedType.PerTestSession)]
public sealed class RoomSeedingTests(AppFixture thisApp)
{
    [Test]
    public async ValueTask Database_ContainsAtLeastTwoRooms(CancellationToken cancellationToken)
    {
        //Arrange
        await thisApp.ResetAsync(cancellationToken);

        //Assert
        await thisApp.ExecuteDbContextAsync(async db =>
        {
            await Assert.That(await db.Rooms.CountAsync(cancellationToken)).Satisfies(count => count >= 2);
        });
    }
}