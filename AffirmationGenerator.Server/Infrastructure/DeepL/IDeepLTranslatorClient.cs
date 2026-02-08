namespace AffirmationGenerator.Server.Infrastructure.DeepL;

public interface IDeepLTranslatorClient
{
    Task<string> Translate(string text, string sourceLanguage, string targetLanguage);
}
