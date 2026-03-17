using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Application.Services.Affirmation;
using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetRemainingAffirmationsQuery(IAffirmationService affirmationService)
{
    public async Task<Result<RemainingAffirmationsResponse>> Handle()
    {
        var remainingCount = await affirmationService.GetRemainingCount();

        var resetTime = await affirmationService.GetResetTime();

        var resetInSeconds = remainingCount == 0 ? resetTime?.TotalSeconds ?? 0 : 0;

        return new RemainingAffirmationsResponse { RemainingCount = remainingCount, ResetInSeconds = resetInSeconds };
    }
}
