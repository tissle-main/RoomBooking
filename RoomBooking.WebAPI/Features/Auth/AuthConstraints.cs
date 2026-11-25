using System.Diagnostics.CodeAnalysis;

namespace RoomBooking.WebAPI.Features.Auth;

public static class AuthConstraints
{
    [StringSyntax(StringSyntaxAttribute.Regex)]
    public const string PassRegex = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)[A-Za-z\\d]{8,}$";
    public const int PasswordMinLength = 8;
}