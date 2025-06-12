using Airbnb.Domain.ValueObject;

namespace Airbnb.Domain.DomainInterfaces;

public interface IBookingConflict
{
    Task<bool> HasConflictAsync(Guid apartmentId, DateRange range);
}