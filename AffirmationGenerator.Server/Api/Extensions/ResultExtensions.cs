using System.Net;
using AffirmationGenerator.Server.Api.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;
using Microsoft.AspNetCore.Mvc;

namespace AffirmationGenerator.Server.Api.Extensions;

public static class ResultExtensions
{
    extension<T>(Task<Result<T>> result)
    {
        public async Task<ActionResult<T>> ToActionResult() =>
            await result switch
            {
                Error<T> { Details: AffirmationNotFound } error => new NotFoundObjectResult(error.ToErrorResponse()),
                Error<T> { Details: TranslationError } error => error.ToObjectResult(HttpStatusCode.InternalServerError),
                Error<T> error => new BadRequestObjectResult(error.ToErrorResponse()),
                Success<T> success => typeof(T) == typeof(Unit) ? new OkResult() : new OkObjectResult(success.Value),
                _ => throw new ArgumentOutOfRangeException(nameof(result)),
            };
    }

    extension<T>(Error<T> error)
    {
        private ObjectResult ToObjectResult(HttpStatusCode statusCode) => new(error.ToErrorResponse()) { StatusCode = (int)statusCode };

        private ErrorResponse ToErrorResponse() =>
            new()
            {
                Details = error.Details switch
                {
                    AffirmationNotFound => "No affirmation available for today :(",
                    TranslationError => "Translation failed. English version will be used instead",
                    _ => error.Details.GetType().Name,
                },
            };
    }
}
