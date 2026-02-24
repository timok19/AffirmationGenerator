using System.ComponentModel;
using System.Text.Json.Serialization;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record GetAffirmationRequest
{
    [Description(
        $"""
            Available language:
            {nameof(AffirmationLanguage.English)} = 0,
            {nameof(AffirmationLanguage.German)} = 1,
            {nameof(AffirmationLanguage.Czech)} = 2,
            {nameof(AffirmationLanguage.French)} = 3
            """
    )]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required AffirmationLanguage TargetLanguage { get; init; }
}
