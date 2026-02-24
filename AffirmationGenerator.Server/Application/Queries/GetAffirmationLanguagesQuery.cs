using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetAffirmationLanguagesQuery
{
    public Result<AffirmationLanguagesResponse> Handle() => ToResponse(GetLanguages());

    private static List<AffirmationLanguage> GetLanguages() => Enum.GetValues<AffirmationLanguage>().ToList();

    private static AffirmationLanguagesResponse ToResponse(List<AffirmationLanguage> languages) => new() { Languages = languages };
}
