using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;

namespace Shared.Redis.Redis;

public class RedisLock : IRedisLock
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan DefaultWait = TimeSpan.Zero;
    private static readonly TimeSpan DefaultRetry = TimeSpan.FromMicroseconds(200);
    private readonly RedLockFactory _factory;
    private readonly ILogger<RedisLock> _logger;

    public RedisLock(RedLockFactory factory, ILogger<RedisLock> logger)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _logger = logger;
    }

    public async Task<IRedLock> LockAsync(string key, TimeSpan? expiry = null,
        TimeSpan? waitTime = null, TimeSpan? retryTime = null)
    {
        var expiryTime = expiry ?? DefaultExpiry;
        var wait = waitTime ?? DefaultWait;
        var retry = retryTime ?? DefaultRetry;
        
        var redLock = await _factory.CreateLockAsync(
            resource: key,
            expiryTime: expiryTime,
            waitTime: wait,
            retryTime: retry);
        
        if (redLock.IsAcquired)
        {
            _logger.LogInformation("Lock acquired for key: {Key}, expires at: {Expiry}", 
                key, DateTime.UtcNow.Add(expiryTime));
        }
        else
        {
            _logger.LogWarning("Failed to acquire lock for key: {Key}", key);
            throw new InvalidOperationException($"Cannot create a lock with key: {key}.");
        }
        
        return redLock;
    }
}