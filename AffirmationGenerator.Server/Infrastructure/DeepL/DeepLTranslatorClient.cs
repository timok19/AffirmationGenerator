using DeepL;
using Microsoft.Extensions.Options;

namespace AffirmationGenerator.Server.Infrastructure.DeepL;

public sealed class DeepLTranslatorClient(IOptions<DeepLTranslatorClientOptions> options, ILogger<DeepLTranslatorClient> logger)
    : IDeepLTranslatorClient
{
    private DeepLTranslatorClientOptions Options => options.Value;

    public async Task<string> Translate(string text, string sourceLanguageCode, string targetLanguageCode)
    {
        if (string.IsNullOrWhiteSpace(sourceLanguageCode) || string.IsNullOrWhiteSpace(targetLanguageCode))
        {
            logger.LogError("Unable to translate text. Source or target language is not set");
            return string.Empty;
        }

        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        using var client = GetDeepLClient();

        try
        {
            var textResult = await client.TranslateTextAsync(text, sourceLanguageCode, targetLanguageCode);

            if (logger.IsEnabled(LogLevel.Information))
                logger.LogInformation("Billed characters {BilledCharacters}", textResult.BilledCharacters);

            return textResult.Text;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected error happened during translation");
        }

        return string.Empty;
    }

    private DeepLClient GetDeepLClient() => new(Options.ApiKey);
}
