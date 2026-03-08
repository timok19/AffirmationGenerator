using AffirmationGenerator.Server.Infrastructure.Affirmation;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using AffirmationGenerator.Server.Infrastructure.Redis;
using Microsoft.Extensions.Options;
using Refit;
using StackExchange.Redis;

namespace AffirmationGenerator.Server.Infrastructure;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("Infrastructure");
            return services
                .AddDeepLTranslatorClient(configurationSection)
                .AddAffirmationClient(configurationSection)
                .AddRedis(configurationSection);
        }

        private IServiceCollection AddDeepLTranslatorClient(IConfiguration configuration)
        {
            services.Configure<DeepLTranslatorClientOptions>(configuration.GetSection(nameof(DeepLTranslatorClientOptions)));
            services.AddScoped<IDeepLTranslatorClient, DeepLTranslatorClient>();

            return services;
        }

        private IServiceCollection AddAffirmationClient(IConfiguration configuration)
        {
            services.Configure<AffirmationClientOptions>(configuration.GetSection(nameof(AffirmationClientOptions)));
            services
                .AddRefitClient<IAffirmationClient>()
                .ConfigureHttpClient(
                    (serviceProvider, httpClient) =>
                    {
                        var baseUrl = serviceProvider.GetRequiredService<IOptions<AffirmationClientOptions>>().Value.BaseUrl;
                        httpClient.BaseAddress = new Uri(baseUrl);
                    }
                );
            return services;
        }

        private IServiceCollection AddRedis(IConfiguration configuration)
        {
            var connectionString =
                configuration.GetSection(nameof(RedisClientOptions)).GetValue<string>(nameof(RedisClientOptions.ConnectionString))
                ?? string.Empty;

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(connectionString));
            services.AddScoped<IRedisClient, RedisClient>();

            return services;
        }
    }
}
