using RoomBooking.WebAPI.Features.Auth.Dtos.Users;

namespace RoomBooking.WebAPI.Features.Auth.Handlers.LoginUser;

public sealed record class LoginUserResponse(UserDto User, string AccessToken);