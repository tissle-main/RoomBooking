# Room Booking

## Run the application

1. Install .NET 11 or later.
2. Install the Aspire CLI: `dotnet tool install -g Aspire.Cli`.
3. Make sure Docker or Podman is running.
4. Configure the Web API's JWT and refresh-token settings (see below).
5. Start the `RoomBooking.AppHost` project.
6. Wait until all resources show `Running`, then open Swagger or Scalar from the Web API resource.

## JWT and refresh-token user secrets

The Web API reads its configuration from the `Jwt` and `RefreshToken` sections. Set these values as user secrets for the Web API project (not the AppHost project):

```powershell
dotnet user-secrets set "Jwt:Issuer" "RoomBooking"
dotnet user-secrets set "Jwt:Audience" "RoomBooking.Client"
dotnet user-secrets set "Jwt:Key" "replace-with-a-random-secret-key-of-at-least-32-characters"
dotnet user-secrets set "Jwt:ExpireMinutes" "15"
dotnet user-secrets set "RefreshToken:ExpireDays" "7"
```

Run the commands from the repository root. User secrets are for local development; use a secure configuration provider for deployed environments. Keep the signing key private and use a strong, randomly generated value.

## Authentication endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Creates an account using the submitted email and password. Returns `204 No Content` when successful. |
| `POST` | `/api/auth/login` | Checks the submitted credentials and returns the user and a JWT access token. Also sets a refresh-token cookie. |
| `PUT` | `/api/auth/refresh-token` | Reads the refresh-token cookie, invalidates the old token, and returns a new access token while rotating the refresh-token cookie. |

The refresh token is stored in an `HttpOnly`, `Secure`, `SameSite=Strict` cookie scoped to `/api/auth/refresh-token`. Call the refresh endpoint over HTTPS and retain the cookie between requests. The access token is returned in the response body and should be sent to protected endpoints as a bearer token.

## Run tests

The solution has separate unit and integration test projects. Run them from the repository root:

```powershell
dotnet run --project .\RoomBooking.UnitTests\RoomBooking.UnitTests.csproj
dotnet run --project .\RoomBooking.IntegrationTests\RoomBooking.IntegrationTests.csproj
```

The integration tests start the Aspire AppHost and require Docker or Podman to be running.