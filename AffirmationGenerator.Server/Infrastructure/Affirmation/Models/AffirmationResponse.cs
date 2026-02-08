namespace AffirmationGenerator.Server.Infrastructure.Affirmation.Models;

public sealed record AffirmationResponse
{
    public required string? Affirmation { get; init; }
}
