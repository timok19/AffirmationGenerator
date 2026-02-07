namespace AffirmationGenerator.Server.Domain;

public sealed record Affirmation
{
    public required Dictionary<AffirmationLanguage, string> Texts { get; init; }
}
