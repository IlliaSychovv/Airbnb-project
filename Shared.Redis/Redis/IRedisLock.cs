using RedLockNet;

namespace Shared.Redis.Redis;

public interface IRedisLock
{
    Task<IRedLock> LockAsync(string key, TimeSpan expiry);
}