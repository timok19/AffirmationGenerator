using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using Microsoft.Extensions.Caching.Memory;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetRemainingAffirmationsQuery(
    IHttpContextAccessor httpContextAccessor,
    IMemoryCache memoryCache,
    ILogger<GetRemainingAffirmationsQuery> logger
)
{
    private string? UserIpAddress => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    private string CacheKey => $"{UserIpAddress}";

    public async Task<Result<RemainingAffirmationsResponse>> Handle()
    {
        var remainingAffirmations = await GetRemainingAffirmations();

        logger.LogInformation("{RemainingAffirmations} affirmations remain for user {UserIpAddress}", remainingAffirmations, UserIpAddress);

        return new RemainingAffirmationsResponse { RemainingAffirmations = remainingAffirmations };
    }

    private async Task<int> GetRemainingAffirmations() =>
        await memoryCache.GetOrCreateAsync(
            CacheKey,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                return Task.FromResult(RateLimitingConstants.MaxRequestsPerIpPerDay);
            }
        );
}
