namespace AffirmationGenerator.Server.Infrastructure.Redis;

public sealed record RedisClientOptions
{
    public required string ConnectionString { get; init; }
}
