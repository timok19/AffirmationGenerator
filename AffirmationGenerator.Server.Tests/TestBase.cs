using AutoFixture;
using Microsoft.Extensions.Logging;

namespace AffirmationGenerator.Server.Tests;

public abstract class TestBase
{
    protected static Fixture Fixture => new();

    protected static ILogger<T> MockLogger<T>() => LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<T>();
}
