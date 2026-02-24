using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record GetAffirmationRequest
{
    public required AffirmationLanguage TargetLanguage { get; init; }
}
