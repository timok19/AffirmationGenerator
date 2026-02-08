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
        var affirmationResponse = await affirmationClient.GetAffirmation();
        var affirmationText = affirmationResponse.Affirmation ?? string.Empty;

        if (string.IsNullOrWhiteSpace(affirmationText))
        {
            logger.LogError("Unable to get affirmation text");
            return Result<AffirmationResponse>.Error(new AffirmationNotFound());
        }

        if (request.TargetLanguage == AffirmationLanguage.English)
            return Result<AffirmationResponse>.Success(new AffirmationResponse(affirmationText));

        var targetLanguageCode = MapLanguageCode(request.TargetLanguage);

        var translation = await translatorClient.Translate(affirmationText, LanguageCode.English, targetLanguageCode);

        return string.IsNullOrWhiteSpace(translation)
            ? Result<AffirmationResponse>.Error(new TranslationError())
            : Result<AffirmationResponse>.Success(new AffirmationResponse(translation));
    }

    private static string MapLanguageCode(AffirmationLanguage language) =>
        language switch
        {
            AffirmationLanguage.English => LanguageCode.English,
            AffirmationLanguage.German => LanguageCode.German,
            AffirmationLanguage.Czech => LanguageCode.Czech,
            AffirmationLanguage.French => LanguageCode.French,
            _ => throw new ArgumentOutOfRangeException(nameof(language)),
        };
}
