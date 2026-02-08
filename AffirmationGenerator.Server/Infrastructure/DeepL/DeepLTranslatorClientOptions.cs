namespace AffirmationGenerator.Server.Infrastructure.DeepL;

public sealed record DeepLTranslatorClientOptions
{
    public required string ApiKey { get; init; }
}
