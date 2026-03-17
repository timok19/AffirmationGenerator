using AffirmationGenerator.Server.Core;

namespace AffirmationGenerator.Server.Application.Services.Affirmation;

public interface IAffirmationService
{
    Task<Result<string>> Get();

    Task<int> GetRemainingCount();

    Task<TimeSpan?> GetResetTime();
}
