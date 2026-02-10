using AffirmationGenerator.Server.Api.Extensions;
using AffirmationGenerator.Server.Api.Models;
using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application.Commands;
using AffirmationGenerator.Server.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AffirmationGenerator.Server.Api.Controllers;

[ApiController]
[Route("affirmations")]
public class AffirmationsController(GenerateAffirmationCommand generateAffirmationCommand) : ControllerBase
{
    [HttpGet]
    [EnableRateLimiting(RateLimitingPolicies.Fixed)]
    [ProducesResponseType<AffirmationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<AffirmationResponse>> Generate([FromQuery] GenerateAffirmationRequest request) =>
        await generateAffirmationCommand.Handle(request).ToActionResult();
}
