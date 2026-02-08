namespace AffirmationGenerator.Server.Application.Models;

public sealed record GenerateAffirmationRequest
{
    public required string AffirmationLanguage { get; init; }
}
