namespace AffirmationGenerator.Server.Application.Models;

public sealed record RemainingAffirmationsResponse
{
    public required int RemainingCount { get; init; }
}
