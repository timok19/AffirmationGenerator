using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record GenerateAffirmationRequest
{
    public required AffirmationLanguage TargetLanguage { get; init; }
}
