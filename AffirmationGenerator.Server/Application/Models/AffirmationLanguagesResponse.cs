namespace AffirmationGenerator.Server.Application.Models;

public sealed record AffirmationLanguagesResponse
{
    public required Dictionary<string, string> Languages { get; init; }
}
