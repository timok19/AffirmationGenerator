using AffirmationGenerator.Server.Infrastructure.Affirmation.Models;
using Refit;

namespace AffirmationGenerator.Server.Infrastructure.Affirmation;

public interface IAffirmationClient
{
    [Get("")]
    Task<AffirmationResponse> GetAffirmation();
}
