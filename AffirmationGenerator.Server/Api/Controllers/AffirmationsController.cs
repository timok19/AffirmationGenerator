using AffirmationGenerator.Server.Api.Extensions;
using AffirmationGenerator.Server.Api.Models;
using AffirmationGenerator.Server.Application.Commands;
using AffirmationGenerator.Server.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace AffirmationGenerator.Server.Api.Controllers;

[ApiController]
[Route("affirmations")]
public class AffirmationsController(GenerateAffirmationCommand generateAffirmationCommand) : ControllerBase
{
    [HttpPost("generate")]
    [ProducesResponseType<AffirmationResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ErrorResponse>(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AffirmationResponse>> Generate([FromBody] GenerateAffirmationRequest request) =>
        await generateAffirmationCommand.Handle(request).ToActionResult();
}
