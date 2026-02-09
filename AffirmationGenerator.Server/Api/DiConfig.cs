using System.Threading.RateLimiting;
using AffirmationGenerator.Server.Api.RateLimiting;

namespace AffirmationGenerator.Server.Api;

public static class DiConfig
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddRateLimiter(rateLimiterOptions =>
        {
            rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            rateLimiterOptions.AddPolicy(
                RateLimitingPolicies.Fixed,
                httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        ResolveRateLimitKey(httpContext),
                        _ => new FixedWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromDays(1),
                            PermitLimit = RateLimitingConstants.MaxRequestsPerDay,
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        }
                    )
            );
        });

        return services;
    }

    private static string ResolveRateLimitKey(HttpContext httpContext)
    {
        var sessionKey = httpContext.Session.GetString(RateLimitingConstants.SessionKey);
        if (string.IsNullOrWhiteSpace(sessionKey) == false)
            return sessionKey;

        sessionKey = httpContext.Session.Id;
        httpContext.Session.SetString(RateLimitingConstants.SessionKey, sessionKey);
        return sessionKey;
    }
}
