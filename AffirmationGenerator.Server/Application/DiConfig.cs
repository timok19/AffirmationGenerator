using AffirmationGenerator.Server.Application.Queries;
using AffirmationGenerator.Server.Application.Services.Affirmation;
using AffirmationGenerator.Server.Application.Services.Language;
using AffirmationGenerator.Server.Domain;

namespace AffirmationGenerator.Server.Application;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication(IConfiguration configuration)
        {
            var configurationSection = configuration.GetSection("Application");
            services.Configure<ClientOptions>(configurationSection.GetSection(nameof(ClientOptions)));

            services.AddHttpContextAccessor();
            services.AddMemoryCache();

            services.AddScoped<GetAffirmationQuery>();
            services.AddScoped<GetRemainingAffirmationsQuery>();
            services.AddScoped<GetAffirmationLanguagesQuery>();

            services.AddScoped<IAffirmationService, AffirmationService>();
            services.AddScoped<ILanguageCodeMapper<AffirmationLanguage>, AffirmationLanguageCodeMapper>();

            return services;
        }
    }
}
