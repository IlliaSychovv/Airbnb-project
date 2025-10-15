using System.Text.Json;
using Microsoft.Extensions.Logging;
using RedLockNet;
using RedLockNet.SERedis;
using StackExchange.Redis;

namespace Shared.Redis.Redis;

public class RedisLock : IRedisLock
{
    private static readonly TimeSpan DefaultExpiry = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan DefaultWait = TimeSpan.Zero;
    private static readonly TimeSpan DefaultRetry = TimeSpan.FromMicroseconds(200);
    private readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(15);
    private readonly RedLockFactory _factory;
    private readonly ILogger<RedisLock> _logger;
    private readonly IDatabase _redis;

    public RedisLock(RedLockFactory factory, ILogger<RedisLock> logger, IDatabase redis)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _logger = logger;
        _redis = redis;
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

    public async Task LockDateRangeForBooking(Guid apartmentId, DateTime startDate, DateTime endDate)
    {
        if (endDate <= startDate)
            throw new ArgumentException("End date must be greater than start date.");

        var redisKey = $"booking:{apartmentId}";
        var lockKey = $"booking-lock:{apartmentId}";

        var lockTaken = await _redis.StringSetAsync(lockKey, "1", _lockTimeout, when: When.NotExists);
        if (!lockTaken)
            throw new InvalidOperationException("Apartment is temporarily locked. Try again.");

        try
        {
            var json = await _redis.StringGetAsync(redisKey);
            var bookings = string.IsNullOrEmpty(json)
                ? new List<(DateTime Start, DateTime End)>()
                : JsonSerializer.Deserialize<List<(DateTime Start, DateTime End)>>(json)!;

            if (bookings.Any(b => startDate < b.End && b.Start < endDate))
            {
                throw new InvalidOperationException("Date range overlaps existing booking.");
            }

            bookings.Add((startDate, endDate));
            await _redis.StringSetAsync(redisKey, JsonSerializer.Serialize(bookings));
        }
        finally
        {
            await _redis.KeyDeleteAsync(lockKey);
        }
    }
}