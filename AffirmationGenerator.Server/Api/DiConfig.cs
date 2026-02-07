namespace AffirmationGenerator.Server.Api;

public static class DiConfig
{
    public static IServiceCollection AddApi(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}
