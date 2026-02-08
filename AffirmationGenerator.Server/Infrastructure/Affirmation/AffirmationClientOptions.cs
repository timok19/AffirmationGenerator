namespace AffirmationGenerator.Server.Infrastructure.Affirmation;

public sealed record AffirmationClientOptions
{
    public required int CountPerDay { get; init; }

    public required string BaseUrl { get; init; }
}
