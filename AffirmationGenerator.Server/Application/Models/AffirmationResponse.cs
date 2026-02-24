using System.Text.Json.Serialization;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record AffirmationResponse
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required AffirmationLanguage TargetLanguage { get; init; }

    public required string Text { get; init; }

    public required int RemainingCount { get; init; }
}
