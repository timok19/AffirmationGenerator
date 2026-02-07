using AffirmationGenerator.Server.Application.Handlers;

namespace AffirmationGenerator.Server.Application;

public static class DiConfig
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetAffirmationHandler>();
        return services;
    }
}
