using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Domain;

public sealed record AffirmationNotFound : ErrorDetails;

public sealed record TranslationError : ErrorDetails;
