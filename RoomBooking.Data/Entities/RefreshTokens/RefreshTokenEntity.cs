using RoomBooking.Data.Entities.Users;

namespace RoomBooking.Data.Entities.RefreshTokens;

public sealed class RefreshTokenEntity
{
    //Value properties
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Value { get; set; } = "";
    public DateTime ExpiresAt { get; set; }
    public Guid UserId { get; set; }

    //Navigation properties
    public UserEntity? User { get; set; }
}