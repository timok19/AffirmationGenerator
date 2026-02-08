using AffirmationGenerator.Server.Infrastructure.Affirmation;
using AffirmationGenerator.Server.Infrastructure.DeepL;
using Microsoft.Extensions.Options;
using Refit;

namespace AffirmationGenerator.Server.Infrastructure;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("Infrastructure");

            return services.AddDeepLTranslatorClient(configurationSection).AddAffirmationClient(configurationSection);
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
    }
}
