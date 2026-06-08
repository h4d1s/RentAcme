using StackExchange.Redis;
using System.Text.Json;

namespace Caching.Services;

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var redisValue = await _db.StringGetAsync(key);

        if (!redisValue.HasValue)
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(redisValue.ToString());
        }
        catch (JsonException)
        {
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, ttl);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }

    public async Task<long> GetVersionAsync(string versionKey)
    {
        var value = await _db.StringGetAsync(versionKey);

        return value.HasValue ? (long)value : 1;
    }

    public async Task<long> IncrementVersionAsync(string versionKey)
    {
        return await _db.StringIncrementAsync(versionKey);
    }
}
