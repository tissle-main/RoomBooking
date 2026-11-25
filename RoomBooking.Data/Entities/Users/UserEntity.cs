using Microsoft.AspNetCore.Identity;
using RoomBooking.Data.Entities.Bookings;
using RoomBooking.Data.Entities.RefreshTokens;

namespace RoomBooking.Data.Entities.Users;

public sealed class UserEntity : IdentityUser<Guid>
{
    //Navigation properties
    public List<RefreshTokenEntity> RefreshTokens { get; set; } = [];
}