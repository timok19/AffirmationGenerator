namespace AffirmationGenerator.Server.Infrastructure.Redis;

public interface IRedisClient
{
    Task<string?> GetString(string key);

    Task<bool> SetString(string key, string value, TimeSpan expiration);

    Task<TimeSpan?> GetKeyTtl(string key);
}
