using RoomBooking.Data.Entities.Users;

namespace RoomBooking.WebAPI.Features.Auth.Services;

public interface IAccessTokenGenerator
{
    public abstract ValueTask<string> GenerateTokenAsync(UserEntity user, CancellationToken cancellationToken);
}