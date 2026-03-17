namespace AffirmationGenerator.Server.Application.Models;

public sealed record RemainingAffirmationsResponse
{
    public required int RemainingCount { get; init; }

    public required double ResetInSeconds { get; init; }
}
