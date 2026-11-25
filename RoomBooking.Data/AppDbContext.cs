using Microsoft.EntityFrameworkCore;
using RoomBooking.Data.Entities.Users;
using RoomBooking.Data.Entities.Roles;
using RoomBooking.Data.Entities.Rooms;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.Data.Entities.RefreshTokens;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RoomBooking.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> thisOptions) : IdentityDbContext<UserEntity, RoleEntity, Guid>(thisOptions)
{
    #region Instance
    public DbSet<RefreshTokenEntity> RefreshTokens
    {
        get => field ??= base.Set<RefreshTokenEntity>();
    }
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
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
    #endregion
}