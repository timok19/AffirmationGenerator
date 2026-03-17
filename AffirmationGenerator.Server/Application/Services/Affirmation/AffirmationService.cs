using AffirmationGenerator.Server.Application.Extensions;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Core.Extensions;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.Affirmation;
using AffirmationGenerator.Server.Infrastructure.Redis;
using Microsoft.Extensions.Options;

namespace AffirmationGenerator.Server.Application.Services.Affirmation;

public sealed class AffirmationService(
    ILogger<AffirmationService> logger,
    IAffirmationClient affirmationClient,
    IRedisClient redisClient,
    IHttpContextAccessor httpContextAccessor,
    IOptions<ClientOptions> clientOptions
) : IAffirmationService
{
    private ClientOptions ClientOptions => clientOptions.Value;

    private string? ClientIpAddress => httpContextAccessor.HttpContext?.GetClientIpAddress(ClientOptions.ClientIpHeaderName);

    private string CacheKey => $"{ClientIpAddress}";

    public async Task<Result<string>> Get()
    {
        var remainingAffirmations = await GetRemainingCount();

        var affirmationResponse = await affirmationClient.GetAffirmation();
        var affirmation = affirmationResponse.Affirmation ?? string.Empty;

        if (string.IsNullOrWhiteSpace(affirmation))
        {
            logger.LogError("Unable to get affirmation text");
            return Result<string>.Error(new AffirmationNotFound());
        }

        await SetRemainingCount(remainingAffirmations);

        return Result<string>.Success(affirmation);
    }

    public async Task<int> GetRemainingCount()
    {
        var cachedValue = await redisClient.GetString(CacheKey);

        if (int.TryParse(cachedValue, out var remainingCount) == false)
        {
            remainingCount = ClientOptions.MaxRequestsPerDay;
            await redisClient.SetString(CacheKey, $"{remainingCount}", TimeSpan.OneDay);
        }

        if (logger.IsEnabled(LogLevel.Information))
            logger.LogInformation("{RemainingCount} affirmations remain for user {ClientIpAddress}", remainingCount, ClientIpAddress);

        return remainingCount;
    }

    public async Task<TimeSpan?> GetResetTime() => await redisClient.GetKeyTtl(CacheKey);

    private async Task SetRemainingCount(int count)
    {
        if (count <= 0)
            return;

        count -= 1;

        if (count <= 0)
            count = 0;

        var resetTime = await GetResetTime() ?? TimeSpan.OneDay;

        await redisClient.SetString(CacheKey, $"{count}", resetTime);
    }
}
