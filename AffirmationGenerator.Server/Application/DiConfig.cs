using AffirmationGenerator.Server.Application.Queries;

namespace AffirmationGenerator.Server.Application;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddHttpContextAccessor();

            services.AddScoped<GetAffirmationQuery>();
            services.AddScoped<GetRemainingAffirmationsQuery>();
            services.AddScoped<GetAffirmationLanguagesQuery>();

            return services;
        }
    }
}
