namespace AffirmationGenerator.Server.Infrastructure.Affirmation;

public sealed record AffirmationClientOptions
{
    public required string BaseUrl { get; init; }
}
