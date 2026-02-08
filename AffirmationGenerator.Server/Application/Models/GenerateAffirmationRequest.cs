namespace AffirmationGenerator.Server.Application.Models;

public sealed record GenerateAffirmationRequest
{
    public required string AffirmationLanguageCode { get; init; }
}
