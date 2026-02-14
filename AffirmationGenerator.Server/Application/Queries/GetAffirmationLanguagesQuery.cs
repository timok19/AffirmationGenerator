using AffirmationGenerator.Server.Application.Models;
using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application.Queries;

public sealed class GetAffirmationLanguagesQuery
{
    public Result<AffirmationLanguagesResponse> Handle() =>
        new AffirmationLanguagesResponse
        {
            Languages = new Dictionary<string, string>
            {
                { AffirmationLanguage.English, nameof(AffirmationLanguage.English) },
                { AffirmationLanguage.German, nameof(AffirmationLanguage.German) },
                { AffirmationLanguage.Czech, nameof(AffirmationLanguage.Czech) },
                { AffirmationLanguage.French, nameof(AffirmationLanguage.French) },
            },
        };
}
