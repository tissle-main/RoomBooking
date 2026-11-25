using Facet;
using RoomBooking.Data.Entities.RefreshTokens;

namespace RoomBooking.WebAPI.Features.Auth.Dtos.RefreshTokens;

[Facet(
    sourceType: typeof(RefreshTokenEntity),
    exclude: [
        nameof(RefreshTokenEntity.Id),
        nameof(RefreshTokenEntity.User),
        nameof(RefreshTokenEntity.UserId)
    ]
)]
public sealed partial class RefreshTokenDto;