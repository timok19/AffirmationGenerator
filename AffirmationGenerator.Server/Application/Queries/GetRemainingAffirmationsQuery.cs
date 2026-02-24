using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Application.Services.Affirmation;
using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetRemainingAffirmationsQuery(IAffirmationService affirmationService)
{
    public async Task<Result<RemainingAffirmationsResponse>> Handle()
    {
        var remainingCount = await affirmationService.Count();
        return new RemainingAffirmationsResponse { RemainingCount = remainingCount };
    }
}
