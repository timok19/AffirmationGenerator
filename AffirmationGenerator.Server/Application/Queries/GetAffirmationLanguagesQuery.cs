using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetAffirmationLanguagesQuery
{
    public Result<AffirmationLanguagesResponse> Handle() => ToResponse(GetLanguages());

    private static Dictionary<string, string> GetLanguages() =>
        new()
        {
            { AffirmationLanguage.English, nameof(AffirmationLanguage.English) },
            { AffirmationLanguage.German, nameof(AffirmationLanguage.German) },
            { AffirmationLanguage.Czech, nameof(AffirmationLanguage.Czech) },
            { AffirmationLanguage.French, nameof(AffirmationLanguage.French) },
        };

    private static AffirmationLanguagesResponse ToResponse(IReadOnlyDictionary<string, string> languages) =>
        new() { Languages = languages.OrderBy(kvp => kvp.Key).ToDictionary(kvp => kvp.Key, kvp => kvp.Value) };
}
