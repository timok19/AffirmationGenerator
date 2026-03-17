using AffirmationGenerator.Server.Core;
using AffirmationGenerator.Server.Domain;
using DeepL;

namespace AffirmationGenerator.Server.Application.Services.Mapping.Language;

public sealed class AffirmationLanguageMapper : IMapper<AffirmationLanguage, string>
{
    public Result<string> Map(AffirmationLanguage value) =>
        value switch
        {
            AffirmationLanguage.English => LanguageCode.English,
            AffirmationLanguage.German => LanguageCode.German,
            AffirmationLanguage.Czech => LanguageCode.Czech,
            AffirmationLanguage.French => LanguageCode.French,
            _ => Result<string>.Error(new InvalidLanguageCode(value.ToString())),
        };
}
