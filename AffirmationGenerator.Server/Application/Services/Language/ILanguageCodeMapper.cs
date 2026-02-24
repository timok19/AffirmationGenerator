using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Application.Services.Language;

public interface ILanguageCodeMapper<in T>
{
    Result<string> Map(T language);
}
