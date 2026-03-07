using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using AffirmationGenerator.Server.Api.Models;
using AffirmationGenerator.Server.Api.RateLimiting;
using AffirmationGenerator.Server.Application;
using AffirmationGenerator.Server.Application.Extensions;
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
            services.ConfigureHttpJsonOptions(options =>
            {
                options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

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
                        var clientOptions = httpContext.RequestServices.GetRequiredService<IOptions<ClientOptions>>().Value;

                        var clientIp = httpContext.GetClientIpAddress(clientOptions.ClientIpHeaderName);

                        return RateLimitPartition.GetFixedWindowLimiter(
                            clientIp,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                Window = TimeSpan.FromDays(1),
                                PermitLimit = clientOptions.MaxRequestsPerDay,
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
            var retryTime = (TimeProvider.System.GetUtcNow() + retryAfter).ToString("R");

            context.HttpContext.Response.Headers.RetryAfter = retryTime;

            var errorDetails = $"Too many requests. Please try again after {retryTime}.";

            await context.HttpContext.Response.WriteAsJsonAsync(new ErrorResponse { Details = errorDetails }, token);
        }
    }
}
