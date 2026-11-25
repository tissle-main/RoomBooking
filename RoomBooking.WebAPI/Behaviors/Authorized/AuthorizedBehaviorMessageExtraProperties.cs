using RoomBooking.Data.Entities.Users;

namespace RoomBooking.WebAPI.Behaviors.Authorized;

public sealed class AuthorizedBehaviorMessageExtraProperties
{
    public UserEntity User { get; set; } = null!;
}