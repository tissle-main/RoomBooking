using ErrorOr;
using Microsoft.AspNetCore.Identity;

namespace RoomBooking.WebAPI.Features.Auth;

public static class AuthErrors
{
    public const string UserExistsErrorCode = "Auth.UserExists";
    public const string UserNotFoundErrorCode = "Auth.UserNotFound";
    public const string UserLockedOutErrorCode = "Auth.UserLockedOut";
    public const string UserNotAllowedErrorCode = "Auth.UserNotAllowed";
    public const string Requires2FAErrorCode = "Auth.Requires2FA";
    public const string InvalidPasswordErrorCode = "Auth.InvalidPassword";
    public const string InvalidPasswordMessage = "Password must contain at least 8 characters, including uppercase, lowercase, and a number.";

    public static Error UserExists(string email)
    {
        return Error.Conflict(UserExistsErrorCode, $"User with email '{email}' already exists.");
    }
    public static Error UserNotFound(string email)
    {
        return Error.NotFound(UserNotFoundErrorCode, $"User with email '{email}' not found.");
    }
    public static Error UserLockedOut()
    {
        return Error.Failure(UserLockedOutErrorCode, "User locked out.");
    }
    public static Error UserNotAllowed()
    {
        return Error.Failure(UserNotAllowedErrorCode, "User not allowed to sign in.");
    }
    public static Error Requires2FA()
    {
        return Error.Failure(Requires2FAErrorCode, "Sign in requires 2FA.");
    }
    public static Error InvalidPassword()
    {
        return Error.Failure(InvalidPasswordErrorCode, "Invalid password.");
    }

    public static IEnumerable<Error> ToErrors(this IdentityResult result)
    {
        if(result.Succeeded)
        {
            return [];
        }
        return result.Errors.Select(error => Error.Failure(error.Code, error.Description));
    }
    public static Error ToError(this SignInResult result)
    {
        if(result.IsLockedOut)
        {
            return UserLockedOut();
        }
        if(result.IsNotAllowed)
        {
            return UserNotAllowed();
        }
        if(result.RequiresTwoFactor)
        {
            return Requires2FA();
        }
        return InvalidPassword();
    }
}