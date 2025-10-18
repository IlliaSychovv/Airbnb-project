using System.Text.Json;
using Airbnb.Application.Interfaces.Providers;
using StackExchange.Redis;

namespace Airbnb.Application.Providers;

public class BookingDateRangeLockProvider : IBookingDateRangeLockProvider
{
    private static readonly TimeSpan _lockTimeout = TimeSpan.FromSeconds(15);
    private readonly IDatabase _redis;

    public BookingDateRangeLockProvider(IDatabase redis)
    {
        _redis = redis;
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