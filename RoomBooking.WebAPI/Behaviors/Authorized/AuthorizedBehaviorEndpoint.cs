namespace RoomBooking.WebAPI.Behaviors.Authorized;

public static class AuthorizedBehaviorEndpoint
{
    public static RouteHandlerBuilder AddAuthorizedBehaviorProductionProblems(this RouteHandlerBuilder builder)
    {
        builder.ProducesProblem(StatusCodes.Status401Unauthorized);
        return builder.ProducesProblem(StatusCodes.Status403Forbidden);
    }
}