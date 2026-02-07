using AffirmationGenerator.Server.Api;
using AffirmationGenerator.Server.Application;
using AffirmationGenerator.Server.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddApi().AddApplication().AddInfrastructure();

// Configure the HTTP request pipeline.
var app = builder.Build();

if (app.Environment.IsProduction() == false)
{
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
