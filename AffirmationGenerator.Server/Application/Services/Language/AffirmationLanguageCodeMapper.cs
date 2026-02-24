using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;
using DeepL;

namespace AffirmationGenerator.Server.Application.Services.Language;

public sealed class AffirmationLanguageCodeMapper : ILanguageCodeMapper<AffirmationLanguage>
{
    public Result<string> Map(AffirmationLanguage language) =>
        language switch
        {
            AffirmationLanguage.English => LanguageCode.English,
            AffirmationLanguage.German => LanguageCode.German,
            AffirmationLanguage.Czech => LanguageCode.Czech,
            AffirmationLanguage.French => LanguageCode.French,
            _ => Result<string>.Error(new InvalidLanguageCode(language.ToString())),
        };
}
