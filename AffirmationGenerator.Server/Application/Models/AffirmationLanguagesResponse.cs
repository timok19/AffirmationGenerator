using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record AffirmationLanguagesResponse
{
    public required List<AffirmationLanguage> Languages { get; init; }
}
