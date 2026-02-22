using AffirmationGenerator.Server;
using AffirmationGenerator.Server.Api;
using AffirmationGenerator.Server.Application;
using AffirmationGenerator.Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ServerOptions>(builder.Configuration.GetSection(nameof(ServerOptions)));

builder.Services.AddApi().AddApplication().AddInfrastructure(builder.Configuration);

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsProduction() == false)
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseDefaultFiles();

app.UseStaticFiles();

app.UseForwardedHeaders();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
