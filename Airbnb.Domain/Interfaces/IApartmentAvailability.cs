using Airbnb.Domain.ValueObject;

namespace Airbnb.Domain.DomainInterfaces;

public interface IApartmentAvailability
{
    Task<bool> IsAvailableAsync(Guid apartmentId  ,DateRange range);
}