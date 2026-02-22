using AffirmationGenerator.Server.Api.Extensions;
using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetRemainingAffirmationsQuery(
    IHttpContextAccessor httpContextAccessor,
    IMemoryCache memoryCache,
    ILogger<GetRemainingAffirmationsQuery> logger,
    IOptions<ServerOptions> httpContextOptions
)
{
    private string? ClientIpAddress =>
        httpContextAccessor.HttpContext?.GetClientIpFromHeaderOrDefault(httpContextOptions.Value.ClientIpHeaderName);

    private string CacheKey => $"{ClientIpAddress}";

    public async Task<Result<RemainingAffirmationsResponse>> Handle()
    {
        var remainingAffirmations = await GetRemainingAffirmations();

        logger.LogInformation("{Remaining} affirmations remain for user {ClientIpAddress}", remainingAffirmations, ClientIpAddress);

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
