using System.Text.Json;
using Airbnb.Application.Interfaces.Services;
using Airbnb.Application.Options;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Airbnb.Infrastructure.RedisServices;

public class RedisService : IRedisService
{
    private readonly ConnectionMultiplexer _redis;
    private readonly IDatabase _db;

    public RedisService(IOptions<RedisSettingsOption> options)
    {
        _redis = ConnectionMultiplexer.Connect(options.Value.ConnectionString);
        _db = _redis.GetDatabase();
    }

    public async Task<string> GetDataAsync(string key)
    {
        return await _db.StringGetAsync(key);
    }

    public async Task SetDataAsync(string key, string value, TimeSpan? expiry = null)
    {
        await _db.StringSetAsync(key, value, expiry);
    }

    public async Task<T> GetAsync<T>(string key)
    {
        var data = await GetDataAsync(key);
        return string.IsNullOrEmpty(data) ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await SetDataAsync(key, json, expiry);
    }

    public async Task DeleteDataAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}