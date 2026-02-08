using DeepL;
using Microsoft.Extensions.Options;

namespace AffirmationGenerator.Server.Infrastructure.DeepL;

public sealed class DeepLTranslatorClient(IOptions<DeepLTranslatorClientOptions> options, ILogger<DeepLTranslatorClient> logger)
    : IDeepLTranslatorClient
{
    private DeepLTranslatorClientOptions TranslatorClientOptions => options.Value;

    public async Task<string> Translate(string text, string sourceLanguage, string targetLanguage)
    {
        if (string.IsNullOrWhiteSpace(targetLanguage))
        {
            logger.LogError("Unable to translate text. Target language is not set");
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        using var client = GetDeepLClient();

        try
        {
            var textResult = await client.TranslateTextAsync(text, sourceLanguage, targetLanguage);

            logger.LogInformation(
                "Billed characters {BilledCharacters} and model type used {ModelTypeUsed}",
                textResult.BilledCharacters,
                textResult.ModelTypeUsed
            );

            return textResult.Text;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected error happened during translation");
        }

        return string.Empty;
    }

    private DeepLClient GetDeepLClient() => new(TranslatorClientOptions.ApiKey);
}
