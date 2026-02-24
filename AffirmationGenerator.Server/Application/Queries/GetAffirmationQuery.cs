using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Application.Services.Affirmation;
using AffirmationGenerator.Server.Application.Services.Language;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Core.Extensions;
using AffirmationGenerator.Server.Domain;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using DeepL;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetAffirmationQuery(
    IDeepLTranslatorClient translatorClient,
    IAffirmationService affirmationService,
    ILanguageCodeMapper<AffirmationLanguage> languageCodeMapper
)
{
    public async Task<Result<AffirmationResponse>> Handle(GetAffirmationRequest request) =>
        await (
            from targetLanguageCode in languageCodeMapper.Map(request.TargetLanguage)
            from affirmation in affirmationService.Get()
            from translatedAffirmation in Translate(targetLanguageCode, affirmation)
            select ToResponse(request.TargetLanguage, translatedAffirmation)
        );

    private async Task<Result<string>> Translate(string targetLanguageCode, string affirmation)
    {
        if (targetLanguageCode == LanguageCode.English)
            return Result<string>.Success(affirmation);

        var translatedAffirmation = await translatorClient.Translate(affirmation, LanguageCode.English, targetLanguageCode);

        return string.IsNullOrWhiteSpace(translatedAffirmation) == false
            ? Result<string>.Success(translatedAffirmation)
            : Result<string>.Error(new TranslationError());
    }

    private async Task<AffirmationResponse> ToResponse(AffirmationLanguage targetLanguage, string affirmation) =>
        new()
        {
            TargetLanguage = targetLanguage,
            Text = affirmation,
            RemainingCount = await affirmationService.Count(),
        };
}
