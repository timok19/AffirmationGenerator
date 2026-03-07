using AffirmationGenerator.Server.Application.Extensions;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.Affirmation;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace AffirmationGenerator.Server.Application.Services.Affirmation;

public sealed class AffirmationService(
    ILogger<AffirmationService> logger,
    IAffirmationClient affirmationClient,
    IMemoryCache memoryCache,
    IHttpContextAccessor httpContextAccessor,
    IOptions<ClientOptions> clientOptions
) : IAffirmationService
{
    private ClientOptions ClientOptions => clientOptions.Value;

    private string? ClientIpAddress => httpContextAccessor.HttpContext?.GetClientIpAddress(ClientOptions.ClientIpHeaderName);

    private string CacheKey => $"{ClientIpAddress}";

    private static TimeSpan OneDay => TimeSpan.FromDays(1);

    public async Task<Result<string>> Get()
    {
        var remainingAffirmations = await Count();

        var affirmationResponse = await affirmationClient.GetAffirmation();
        var affirmation = affirmationResponse.Affirmation ?? string.Empty;

        if (string.IsNullOrWhiteSpace(affirmation))
        {
            logger.LogError("Unable to get affirmation text");
            return Result<string>.Error(new AffirmationNotFound());
        }

        SetCount(remainingAffirmations);

        return Result<string>.Success(affirmation);
    }

    public async Task<int> Count()
    {
        var remainingCount = await memoryCache.GetOrCreateAsync(
            CacheKey,
            entry =>
            {
                entry.SetAbsoluteExpiration(OneDay);
                return Task.FromResult(ClientOptions.MaxRequestsPerDay);
            }
        );

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("{RemainingCount} affirmations remain for user {ClientIpAddress}", remainingCount, ClientIpAddress);

        return remainingCount;
    }

    private void SetCount(int count)
    {
        if (count <= 0)
            return;

        count -= 1;

        if (count <= 0)
            count = 0;

        memoryCache.Set(CacheKey, count, OneDay);
    }
}
