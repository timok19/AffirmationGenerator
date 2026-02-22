using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Core.Extensions;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.Affirmation;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using DeepL;
using Microsoft.Extensions.Caching.Memory;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetAffirmationQuery(
    IAffirmationClient affirmationClient,
    IDeepLTranslatorClient translatorClient,
    ILogger<GetAffirmationQuery> logger,
    IHttpContextAccessor httpContextAccessor,
    IMemoryCache memoryCache
)
{
    private string? UserIpAddress => httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    private string CacheKey => $"{UserIpAddress}";

    public async Task<Result<AffirmationResponse>> Handle(GetAffirmationRequest request) =>
        await (
            from targetLanguageCode in MapLanguageCode(request.LanguageCode)
            from affirmation in GetAffirmation()
            from translatedAffirmation in Translate(targetLanguageCode, affirmation)
            select ToResponse(targetLanguageCode, translatedAffirmation)
        );

    private static Result<string> MapLanguageCode(string languageCode) =>
        languageCode switch
        {
            AffirmationLanguage.English => LanguageCode.English,
            AffirmationLanguage.German => LanguageCode.German,
            AffirmationLanguage.Czech => LanguageCode.Czech,
            AffirmationLanguage.French => LanguageCode.French,
            _ => Result<string>.Error(new InvalidLanguageCode(languageCode)),
        };

    private async Task<Result<string>> GetAffirmation()
    {
        var remainingAffirmations = await GetRemainingAffirmations();

        var affirmationResponse = await affirmationClient.GetAffirmation();
        var affirmation = affirmationResponse.Affirmation ?? string.Empty;

        if (string.IsNullOrWhiteSpace(affirmation))
        {
            logger.LogError("Unable to get affirmation text");
            return Result<string>.Error(new AffirmationNotFound());
        }

        SetRemainingAffirmations(remainingAffirmations);

        return Result<string>.Success(affirmation);
    }

    private async Task<Result<string>> Translate(string targetLanguageCode, string affirmation)
    {
        if (targetLanguageCode == LanguageCode.English)
            return affirmation;

        var translatedAffirmation = await translatorClient.Translate(affirmation, LanguageCode.English, targetLanguageCode);

        return string.IsNullOrWhiteSpace(translatedAffirmation) == false
            ? Result<string>.Success(translatedAffirmation)
            : Result<string>.Error(new TranslationError());
    }

    private async Task<AffirmationResponse> ToResponse(string targetLanguageCode, string affirmation) =>
        new(targetLanguageCode, affirmation, await GetRemainingAffirmations());

    private async Task<int> GetRemainingAffirmations() =>
        await memoryCache.GetOrCreateAsync(
            CacheKey,
            entry =>
            {
                entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                return Task.FromResult(RateLimitingConstants.MaxRequestsPerIpPerDay);
            }
        );

    private void SetRemainingAffirmations(int remainingAffirmations)
    {
        if (remainingAffirmations <= 0)
            return;

        remainingAffirmations -= 1;

        if (remainingAffirmations <= 0)
            remainingAffirmations = 0;

        logger.LogInformation("{RemainingAffirmations} affirmations remain for user {UserIpAddress}", remainingAffirmations, UserIpAddress);

        memoryCache.Set(CacheKey, remainingAffirmations, TimeSpan.FromDays(1));
    }
}
