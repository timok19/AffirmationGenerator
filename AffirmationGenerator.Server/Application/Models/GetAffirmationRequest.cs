using System.ComponentModel;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Models;

public sealed record GetAffirmationRequest
{
    [Description(
        $"""
            Available language codes: 
            {AffirmationLanguage.English} ({nameof(AffirmationLanguage.English)}), 
            {AffirmationLanguage.German} ({nameof(AffirmationLanguage.German)}), 
            {AffirmationLanguage.Czech} ({nameof(AffirmationLanguage.Czech)}), 
            {AffirmationLanguage.French} ({nameof(AffirmationLanguage.French)}) 
            """
    )]
    public required string LanguageCode { get; init; }
}
