using StackExchange.Redis;

namespace AffirmationGenerator.Server.Infrastructure.Redis;

public sealed class RedisClient(IConnectionMultiplexer redis) : IRedisClient
{
    private IDatabase Database => redis.GetDatabase();

    public async Task<string?> GetString(string key)
    {
        var value = await Database.StringGetAsync(GetPrefixKey(key));
        return value.HasValue == false ? null : value.ToString();
    }

    public async Task<bool> SetString(string key, string value, TimeSpan expiration) =>
        await Database.StringSetAsync(GetPrefixKey(key), value, expiration);

    public async Task<TimeSpan?> GetKeyTtl(string key) => await Database.KeyTimeToLiveAsync(GetPrefixKey(key));

    private static string GetPrefixKey(string key) => $"{nameof(RedisClient)}:{key}";
}
