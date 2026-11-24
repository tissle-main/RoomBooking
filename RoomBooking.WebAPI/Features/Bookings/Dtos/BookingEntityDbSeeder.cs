using Bogus;
using RoomBooking.Data;
using RoomBooking.Data.Entities.Bookings;

namespace RoomBooking.WebAPI.Features.Bookings.Dtos;

public static class BookingEntityDbSeeder
{
    private static Faker Faker { get; } = new();

    extension(Faker<BookingEntity> thisFaker)
    {
        public async ValueTask<List<BookingEntity>> SeedDatabaseAsync(
            AppDbContext dbContext,
            CancellationToken cancellationToken,
            int min = 2,
            int max = 5
        )
        {
            int count = Faker.Random.Number(min, max);
            DateTimeOffset currentOffset = DateTimeOffset.UtcNow;
            List<BookingEntity> bookings = [];
            for(int index = 0; index < count; index++)
            {
                Faker<BookingEntity> faker = thisFaker.Clone();
                BookingEntity entity = faker.WithBookingPeriod(currentOffset.AddDays(2 * index)).Generate();
                await dbContext.Bookings.AddAsync(entity, cancellationToken);
                bookings.Add(entity);
            }
            await dbContext.SaveChangesAsync(cancellationToken);
            return bookings;
        }
    }
}