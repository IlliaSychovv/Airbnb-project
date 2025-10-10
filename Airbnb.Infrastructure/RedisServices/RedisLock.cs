using Airbnb.Application.Interfaces;
using RedLockNet;
using RedLockNet.SERedis;

namespace Airbnb.Infrastructure.RedisServices;

public class RedisLock : IRedisLock
{
    private readonly RedLockFactory _factory;

    public RedisLock(RedLockFactory factory)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    public async Task<IRedLock> LockAsync(string key, TimeSpan expiry)
    {
        var redLock = await _factory.CreateLockAsync(key, expiry);
        if (!redLock.IsAcquired)
            throw new InvalidOperationException($"Cannot create a lock with key: {key}.");
        
        return redLock;
    }
}