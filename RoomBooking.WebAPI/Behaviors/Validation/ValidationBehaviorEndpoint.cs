namespace RoomBooking.WebAPI.Behaviors.Validation;

public static class ValidationBehaviorEndpoint
{
    public static RouteHandlerBuilder AddValidationBehaviorProductionProblems(this RouteHandlerBuilder builder)
    {
        return builder.ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity);
    }
}