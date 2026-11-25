using Facet;
using RoomBooking.Data.Entities.Users;

namespace RoomBooking.WebAPI.Features.Auth.Dtos.Users;

[Facet(
    sourceType: typeof(UserEntity),
    Include = [
        nameof(UserEntity.Email)
    ]
)]
public sealed partial class UserDto;