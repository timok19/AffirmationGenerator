using AffirmationGenerator.Server.Api.Extensions;
using AffirmationGenerator.Server.Api.Models;
using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Application.Queries;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AffirmationGenerator.Server.Api.Controllers;

[ApiController]
[Route("affirmations")]
public class AffirmationsController(
    GetAffirmationQuery getAffirmationQuery,
    GetRemainingAffirmationsQuery getRemainingAffirmationsQuery,
    GetAffirmationLanguagesQuery getAffirmationLanguagesQuery
) : ControllerBase
{
    [HttpGet]
    [EnableRateLimiting(RateLimitingPolicies.Fixed)]
    [ProducesResponseType<AffirmationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AffirmationResponse>> GetAffirmation([FromQuery] GetAffirmationRequest request) =>
        await getAffirmationQuery.Handle(request).ToActionResult();

    [HttpGet("remaining")]
    [ProducesResponseType<RemainingAffirmationsResponse>(StatusCodes.Status200OK)]
    public ActionResult<RemainingAffirmationsResponse> GetRemaining() => getRemainingAffirmationsQuery.Handle().ToActionResult();

    [HttpGet("languages")]
    [ProducesResponseType<AffirmationLanguagesResponse>(StatusCodes.Status200OK)]
    public ActionResult<AffirmationLanguagesResponse> GetLanguages() => getAffirmationLanguagesQuery.Handle().ToActionResult();
}
