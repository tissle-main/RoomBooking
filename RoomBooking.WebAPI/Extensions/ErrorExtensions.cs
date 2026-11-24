using ErrorOr;
using Mediator;
using Microsoft.AspNetCore.Mvc;

namespace RoomBooking.WebAPI.Extensions;

public static class ErrorExtensions
{
    public static ProblemDetails ToProblemDetails(this Error error)
    {
        return new ProblemDetails()
        {
            Status = error.Type switch
            {
                ErrorType.Failure => StatusCodes.Status400BadRequest,
                ErrorType.Validation => StatusCodes.Status422UnprocessableEntity,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            },
            Title = error.Code,
            Detail = error.Description,
        };
    }
    public static ProblemDetails ToProblemDetails(this List<Error>? errors)
    {
        if(errors is null || errors.Count == 0)
        {
            errors = [Error.Unexpected()];
        }
        if(errors.All(error => error.Type is ErrorType.Validation))
        {
            Error validationError = Error.Validation();
            return new HttpValidationProblemDetails()
            {
                Status = StatusCodes.Status422UnprocessableEntity,
                Title = validationError.Code,
                Detail = validationError.Description,
                Errors = errors.Select(error =>
                {
                    return new KeyValuePair<string, string[]>(error.Code, [error.Description]);
                }).ToDictionary()
            };
        }
        ProblemDetails problemDetails = errors[0].ToProblemDetails();
        problemDetails.Extensions = errors.Skip(1).Select((error, index) =>
        {
            return new KeyValuePair<string, object?>(index.ToString(), error.ToProblemDetails());
        }).ToDictionary();
        return problemDetails;
    }
    public static IResult ToHttpResult<TValue>(this ErrorOr<TValue> errorOrValue)
    {
        if(errorOrValue.IsSuccess)
        {
            return typeof(TValue) == typeof(Unit) ? Results.NoContent() : Results.Ok(errorOrValue.Value);
        }
        return Results.Problem(errorOrValue.Errors.ToProblemDetails());
    }
}