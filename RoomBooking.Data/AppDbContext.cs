using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Rooms;
using RoomBooking.Data.Entities.Bookings;

namespace RoomBooking.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> thisOptions) : DbContext(thisOptions)
{
    #region Instance
    public DbSet<RoomEntity> Rooms
    {
        get => field ??= base.Set<RoomEntity>();
    }
    public DbSet<BookingEntity> Bookings
    {
        get => field ??= base.Set<BookingEntity>();
    }
    #endregion

    #region Base
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    #endregion
}