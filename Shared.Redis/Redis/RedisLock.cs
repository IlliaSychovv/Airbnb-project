using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;

namespace Shared.Redis.Redis;

public class RedisLock : IRedisLock
{
    private readonly RedLockFactory _factory;
    private readonly ILogger<RedisLock> _logger;

    public RedisLock(RedLockFactory factory, ILogger<RedisLock> logger)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _logger = logger;
    }

    public async Task<IRedLock> LockAsync(string key, TimeSpan expiry)
    {
        var redLock = await _factory.CreateLockAsync(key, expiry);
        
        if (redLock.IsAcquired)
        {
            _logger.LogInformation("Lock acquired for key: {Key}, expires at: {Expiry}", 
                key, DateTime.UtcNow.Add(expiry));
        }
        else
        {
            _logger.LogWarning("Failed to acquire lock for key: {Key}", key);
            throw new InvalidOperationException($"Cannot create a lock with key: {key}.");
        }
        
        return redLock;
    }
}