using AffirmationGenerator.Server.Application.Commands;

namespace AffirmationGenerator.Server.Application;

public static class DiConfig
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddHttpContextAccessor();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromDays(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.Name = "session";
                options.Cookie.Path = "/";
                options.Cookie.MaxAge = TimeSpan.FromDays(1);
            });

            services.AddScoped<GenerateAffirmationCommand>();
            return services;
        }
    }
}
