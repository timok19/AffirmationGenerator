using AffirmationGenerator.Server.Application.Commands;

namespace AffirmationGenerator.Server.Application;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddScoped<GenerateAffirmationCommand>();
            return services;
        }
    }
}
