using System.Threading.RateLimiting;
using AffirmationGenerator.Server.Api.Authentication;
using Microsoft.AspNetCore.RateLimiting;

namespace AffirmationGenerator.Server.Api;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApi()
        {
            services.AddControllers();
            services.AddOpenApi();

            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddRateLimiter(rateLimiterOptions =>
            {
                rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                rateLimiterOptions.AddFixedWindowLimiter(
                    RateLimitingPolicies.Fixed,
                    options =>
                    {
                        options.Window = TimeSpan.FromDays(1);
                        options.PermitLimit = 5;
                        options.QueueLimit = 0;
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                    }
                );
            });

            return services;
        }
    }
}
