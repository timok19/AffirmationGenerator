using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record AffirmationLanguagesResponse
{
    public required Dictionary<AffirmationLanguage, string> Languages { get; init; }
}
