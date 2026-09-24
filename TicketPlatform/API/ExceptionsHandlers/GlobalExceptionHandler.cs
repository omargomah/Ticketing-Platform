using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace API.ExceptionsHandlers
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            ProblemDetails problemDetails = exception switch
            {
                ValidationException vx => new ValidationProblemDetails(
                vx.Errors.GroupBy(e => e.PropertyName)
                .ToDictionary(e => e.Key, e => e.Select(e => e.ErrorMessage).ToArray()))
                {
                    Title = "Invalid Input",
                    Status = StatusCodes.Status400BadRequest
                },
                _ => new ProblemDetails()
                {
                    Title = "Internal Server Error",
                    Detail = exception.Message,
                    Status = StatusCodes.Status500InternalServerError
                }
            };

            httpContext.Response.StatusCode = problemDetails.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync((object)problemDetails, cancellationToken);
            return true;
        }
    }
}
