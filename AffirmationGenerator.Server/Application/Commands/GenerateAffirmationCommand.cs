using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Core.Extensions;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.Affirmation;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using DeepL;

namespace AffirmationGenerator.Server.Application.Commands;

public sealed class GenerateAffirmationCommand(
    IAffirmationClient affirmationClient,
    IDeepLTranslatorClient translatorClient,
    ILogger<GenerateAffirmationCommand> logger,
    IHttpContextAccessor httpContextAccessor
)
{
    private ISession Session =>
        httpContextAccessor.HttpContext?.Session ?? throw new NullReferenceException($"{nameof(HttpContext)} is missing!");

    private const string RemainingAffirmationsKey = "RemainingAffirmations";

    public async Task<Result<AffirmationResponse>> Handle(GenerateAffirmationRequest request) =>
        await
            from affirmation in GetAffirmation()
            from targetLanguageCode in MapLanguageCode(request.AffirmationLanguageCode)
            from translatedData in Translate(affirmation, targetLanguageCode)
            select ToResponse(translatedData.LanguageCode, translatedData.TranslatedAffirmation);

    private async Task<Result<string>> GetAffirmation()
    {
        var remainingAffirmations = GetRemainingAffirmations();

        var affirmationResponse = await affirmationClient.GetAffirmation();
        var affirmation = affirmationResponse.Affirmation ?? string.Empty;

        if (string.IsNullOrWhiteSpace(affirmation))
        {
            logger.LogError("Unable to get affirmation text");
            return Result<string>.Error(new AffirmationNotFound());
        }

        SetRemainingAffirmations(remainingAffirmations);

        return affirmation;
    }

    private static Result<string> MapLanguageCode(string affirmationLanguageCode) =>
        affirmationLanguageCode switch
        {
            AffirmationLanguage.English => LanguageCode.English,
            AffirmationLanguage.German => LanguageCode.German,
            AffirmationLanguage.Czech => LanguageCode.Czech,
            AffirmationLanguage.French => LanguageCode.French,
            _ => Result<string>.Error(new InvalidLanguageCode(affirmationLanguageCode)),
        };

    private async Task<Result<(string LanguageCode, string TranslatedAffirmation)>> Translate(string affirmation, string targetLanguageCode)
    {
        if (targetLanguageCode == LanguageCode.English)
            return (LanguageCode.English, affirmation);

        var translatedAffirmation = await translatorClient.Translate(affirmation, LanguageCode.English, targetLanguageCode);

        if (string.IsNullOrWhiteSpace(translatedAffirmation))
            return Result<(string, string)>.Error(new TranslationError());

        return (targetLanguageCode, translatedAffirmation);
    }

    private AffirmationResponse ToResponse(string targetLanguageCode, string affirmation) =>
        new(targetLanguageCode, affirmation, GetRemainingAffirmations());

    private int GetRemainingAffirmations() => Session.GetInt32(RemainingAffirmationsKey) ?? RateLimitingConstants.MaxRequestsPerDay;

    private void SetRemainingAffirmations(int remainingAffirmations)
    {
        if (remainingAffirmations <= 0)
            return;

        remainingAffirmations -= 1;
        if (remainingAffirmations <= 0)
            remainingAffirmations = 0;

        Session.SetInt32(RemainingAffirmationsKey, remainingAffirmations);
    }
}
