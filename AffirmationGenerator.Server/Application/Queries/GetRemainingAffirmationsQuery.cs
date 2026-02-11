using AffirmationGenerator.Server.Api.Extensions;
using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetRemainingAffirmationsQuery(IHttpContextAccessor httpContextAccessor)
{
    private ISession Session =>
        httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException($"{nameof(HttpContext)} is missing!");

    public Result<RemainingAffirmationsResponse> Handle()
    {
        var remainingAffirmations = Session.GetInt32(Session.RemainingRequestsKey) ?? RateLimitingConstants.MaxRequestsPerDay;
        return new RemainingAffirmationsResponse { RemainingAffirmations = remainingAffirmations };
    }
}
