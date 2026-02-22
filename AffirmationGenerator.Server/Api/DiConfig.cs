using System.Threading.RateLimiting;
using AffirmationGenerator.Server.Api.Extensions;
using AffirmationGenerator.Server.Api.Models;
using AffirmationGenerator.Server.Api.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace AffirmationGenerator.Server.Api;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApi()
        {
            services.AddControllers();
            services.AddOpenApi();
            services.AddRateLimiting();

            return services;
        }

        private IServiceCollection AddRateLimiting() =>
            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                rateLimiterOptions.OnRejected = OnRejected;
                rateLimiterOptions.AddPolicy(
                    RateLimitingPolicies.Fixed,
                    httpContext =>
                    {
                        var clientIpHeaderName = httpContext
                            .RequestServices.GetRequiredService<IOptions<ServerOptions>>()
                            .Value.ClientIpHeaderName;

                        var clientIp = httpContext.GetClientIpFromHeaderOrDefault(clientIpHeaderName);

                        return RateLimitPartition.GetFixedWindowLimiter(
                            clientIp,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                Window = TimeSpan.FromDays(1),
                                PermitLimit = RateLimitingConstants.MaxRequestsPerIpPerDay,
                            }
                        );
                    }
                );
            });
    }

    private static async ValueTask OnRejected(OnRejectedContext context, CancellationToken token)
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter = $"{retryAfter.TotalSeconds}";

            var errorDetails = $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds.";

            await context.HttpContext.Response.WriteAsJsonAsync(new ErrorResponse { Details = errorDetails }, token);
        }
    }
}
