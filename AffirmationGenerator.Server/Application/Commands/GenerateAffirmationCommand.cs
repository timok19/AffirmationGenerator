using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.Affirmation;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using DeepL;

namespace AffirmationGenerator.Server.Application.Commands;

public sealed class GenerateAffirmationCommand(
    IAffirmationClient affirmationClient,
    IDeepLTranslatorClient translatorClient,
    ILogger<GenerateAffirmationCommand> logger
)
{
    public async Task<Result<AffirmationResponse>> Handle(GenerateAffirmationRequest request)
    {
        var targetLanguageCode = MapLanguageCode(request.AffirmationLanguage);
        if (string.IsNullOrWhiteSpace(targetLanguageCode))
            return Result<AffirmationResponse>.Error(new InvalidLanguageCode(request.AffirmationLanguage));

        var affirmationResponse = await affirmationClient.GetAffirmation();
        var affirmation = affirmationResponse.Affirmation ?? string.Empty;

        if (string.IsNullOrWhiteSpace(affirmation))
        {
            logger.LogError("Unable to get affirmation text");
            return Result<AffirmationResponse>.Error(new AffirmationNotFound());
        }

        if (targetLanguageCode == LanguageCode.English)
            return Result<AffirmationResponse>.Success(
                new AffirmationResponse(new Dictionary<string, string> { [LanguageCode.English] = affirmation })
            );

        var translatedAffirmation = await translatorClient.Translate(affirmation, LanguageCode.English, targetLanguageCode);

        return string.IsNullOrWhiteSpace(translatedAffirmation)
            ? Result<AffirmationResponse>.Error(new TranslationError())
            : Result<AffirmationResponse>.Success(
                new AffirmationResponse(new Dictionary<string, string> { [targetLanguageCode] = translatedAffirmation })
            );
    }

    private static string MapLanguageCode(string affirmationLanguage) =>
        affirmationLanguage switch
        {
            AffirmationLanguage.English => LanguageCode.English,
            AffirmationLanguage.German => LanguageCode.German,
            AffirmationLanguage.Czech => LanguageCode.Czech,
            AffirmationLanguage.French => LanguageCode.French,
            _ => string.Empty,
        };
}
