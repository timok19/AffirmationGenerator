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
        public IServiceCollection AddApi(IConfiguration configuration)
        {
            services.Configure<ApiOptions>(configuration.GetSection(nameof(ApiOptions)));

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
                        var apiOptions = httpContext.RequestServices.GetRequiredService<IOptions<ApiOptions>>().Value;

                        var clientIp = httpContext.GetClientIpFromHeaderOrDefault(apiOptions.ClientIpHeaderName);

                        return RateLimitPartition.GetFixedWindowLimiter(
                            clientIp,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                Window = TimeSpan.FromDays(1),
                                PermitLimit = apiOptions.MaxRequestsPerDay,
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
