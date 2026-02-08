using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Domain;

public sealed record AffirmationNotFound : ErrorDetails;

public sealed record TranslationError : ErrorDetails;

public sealed record InvalidLanguageCode(string LanguageCode) : ErrorDetails;
