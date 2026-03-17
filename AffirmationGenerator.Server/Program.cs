using AffirmationGenerator.Server.Api;
using AffirmationGenerator.Server.Application;
using AffirmationGenerator.Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi().AddApplication(builder.Configuration).AddInfrastructure(builder.Configuration).AddHealthChecks();

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

app.MapHealthChecks("/health");

app.MapFallbackToFile("/index.html");

app.Run();
