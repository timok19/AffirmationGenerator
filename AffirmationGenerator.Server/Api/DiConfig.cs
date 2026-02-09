using System.Threading.RateLimiting;
using AffirmationGenerator.Server.Api.Models;
using AffirmationGenerator.Server.Api.RateLimiting;

namespace AffirmationGenerator.Server.Api;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApi()
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                rateLimiterOptions.OnRejected = async (context, token) =>
                {
                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        context.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";

                        var messageDetails = $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds.";

                        await context.HttpContext.Response.WriteAsJsonAsync(new ErrorResponse { Details = messageDetails }, token);
                    }
                };
                rateLimiterOptions.AddPolicy(
                    RateLimitingPolicies.Fixed,
                    httpContext =>
                        RateLimitPartition.GetFixedWindowLimiter(
                            httpContext.Session.Id,
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
    }
}
