using RedLockNet;

namespace Shared.Redis.Redis;

public interface IRedisLock
{ 
        Task<IRedLock> LockAsync(string key, TimeSpan? expiry = null,
        TimeSpan? waitTime = null, TimeSpan? retryTime = null);
        Task LockDateRangeForBooking(Guid apartmentId, DateTime startDate, DateTime endDate);
}