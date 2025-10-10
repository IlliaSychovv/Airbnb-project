using RedLockNet;

namespace Airbnb.Application.Interfaces;

public interface IRedisLock
{
    Task<IRedLock> LockAsync(string key, TimeSpan expiry);
}