using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetAffirmationLanguagesQuery
{
    public Result<AffirmationLanguagesResponse> Handle() => ToResponse(GetLanguages());

    private static Dictionary<AffirmationLanguage, string> GetLanguages() =>
        Enum.GetValues<AffirmationLanguage>().ToDictionary(language => language, language => language.ToString());

    private static AffirmationLanguagesResponse ToResponse(Dictionary<AffirmationLanguage, string> languages) =>
        new() { Languages = languages };
}
