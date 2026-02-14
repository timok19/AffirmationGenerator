using System.ComponentModel;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record GetAffirmationRequest
{
    [Description("Available languages: en (English), de (German), cs (Czech), fr (French)")]
    public required string AffirmationLanguageCode { get; init; }
}
